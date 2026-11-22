using FitnessApp.Auth;
using FitnessProfileService.Application;

namespace FitnessProfileService.Api;

public sealed class OnboardingGateFilter(IOnboardingGate gate, ICurrentUser current) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var completed = await gate.IsCompletedAsync(current.UserId, context.HttpContext.RequestAborted);
        if (!completed)
        {
            return Results.Problem(
                title: "Onboarding required",
                detail: "Complete the fitness profile onboarding before using this endpoint.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return await next(context);
    }
}

public static class OnboardingGateFilterExtensions
{
    public static RouteHandlerBuilder RequireCompletedOnboarding(this RouteHandlerBuilder builder)
        => builder.AddEndpointFilter<OnboardingGateFilter>();
}
