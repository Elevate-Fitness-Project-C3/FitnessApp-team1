using FitnessApp.Auth;
using FitnessProfileService.Application;

namespace FitnessProfileService.Api;

public static class ProfileEndpoints
{
    public static IEndpointRouteBuilder MapProfileEndpoints(this IEndpointRouteBuilder app)
    {
        var profile = app.MapGroup("/api/profile").RequireAuthorization();

        profile.MapPost("/onboarding", async (
            OnboardingRequest request,
            ProfileService service,
            ICurrentUser current,
            TimeProvider clock,
            CancellationToken ct) =>
        {
            var today = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
            var validation = OnboardingValidator.Validate(request, today);
            if (!validation.IsValid)
                return Results.BadRequest(new { error = validation.Code, detail = validation.Detail });

            var result = await service.OnboardAsync(current.UserId, request, ct);
            return result.Status == ProfileChangeStatus.Conflict
                ? Results.Conflict(new { error = result.Code, detail = result.Detail })
                : Results.Created("/api/profile/me", result.Profile);
        })
        .WithName("CompleteOnboarding");

        profile.MapGet("/dev-check", (ICurrentUser current) =>
            Results.Ok(new { userId = current.UserId, isAdmin = current.IsAdmin }))
            .WithName("ProfileDevCheck");

        var own = profile.MapGroup("/me");

        own.MapGet("/", async (ProfileService service, ICurrentUser current, CancellationToken ct) =>
        {
            var result = await service.GetProfileAsync(current.UserId, ct);
            return result is null
                ? Results.NotFound()
                : Results.Ok(result);
        })
        .RequireCompletedOnboarding()
        .WithName("GetMyProfile");

        own.MapGet("/measurements", async (ProfileService service, ICurrentUser current, CancellationToken ct)
            => Results.Ok(await service.GetMeasurementsAsync(current.UserId, ct)))
            .RequireCompletedOnboarding()
            .WithName("GetMyMeasurements");

        own.MapPost("/measurements", async (
            MeasurementRequest request,
            ProfileService service,
            ICurrentUser current,
            CancellationToken ct) =>
        {
            var validation = OnboardingValidator.ValidateMeasurement(request.HeightCm, request.WeightKg);
            if (!validation.IsValid)
                return Results.BadRequest(new { error = validation.Code, detail = validation.Detail });

            var created = await service.AddMeasurementAsync(current.UserId, request, ct);
            return Results.Created($"/api/profile/me/measurements/{created.MeasurementId}", created);
        })
        .RequireCompletedOnboarding()
        .WithName("AddMyMeasurement");

        return app;
    }
}
