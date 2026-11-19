using FitnessApp.Auth;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Extensions;
using TrainingService.Infrastructure.Persistence;

namespace TrainingService.Features.Authz;

public sealed record CreateExerciseRequest(string? Name);
public sealed record UpdateSessionRequest(SessionStatus Status, DateTime? CompletedAt);

/// <summary>
/// Development/test-only endpoints added for ID-06 authorization coverage.
/// They are not product feature endpoints; real catalog/session endpoints must apply
/// the same policies and the OwnedQueries.ForCurrentUser filter.
/// </summary>
public static class AuthzDevEndpoints
{
    public static IEndpointRouteBuilder MapAuthzDevEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/authz/admin-ping", () => Results.Ok(new { ok = true }))
            .RequireAdmin();

        app.MapGet("/api/authz/user-ping", (ICurrentUser current) =>
            Results.Ok(new { userId = current.UserId, isAdmin = current.IsAdmin }));

        // Reads inherit only the fallback authenticated-user policy.
        var catalogReads = app.MapGroup("/api/dev/catalog");
        catalogReads.MapGet("/exercises", async (TrainingDbContext db, CancellationToken ct) =>
            Results.Ok(await db.Exercises.AsNoTracking().ToListAsync(ct)));

        var catalog = app.MapGroup("/api/dev/catalog").RequireAdmin();

        catalog.MapPost("/exercises", async (CreateExerciseRequest req, TrainingDbContext db, CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(req.Name))
                return Results.BadRequest(new { error = "name_required" });

            var exercise = new Exercise
            {
                Id = Guid.NewGuid(),
                Name = req.Name.Trim(),
                Description = "",
                TargetMuscle = MuscleGroup.FullBody,
                TrackingType = TrackingType.Reps,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            db.Exercises.Add(exercise);
            await db.SaveChangesAsync(ct);
            return Results.Created($"/api/dev/catalog/exercises/{exercise.Id}", new { exercise.Id });
        });

        var sessions = app.MapGroup("/api/dev/sessions");

        sessions.MapGet("/", async (ICurrentUser current, TrainingDbContext db, CancellationToken ct) =>
        {
            var items = await db.WorkoutSessions
                .AsNoTracking()
                .ForCurrentUser(current)
                .Where(x => !x.IsDeleted)
                .ToListAsync(ct);

            return Results.Ok(items);
        });

        sessions.MapGet("/{id:guid}", async (Guid id, ICurrentUser current, TrainingDbContext db, CancellationToken ct) =>
        {
            var session = await db.WorkoutSessions
                .AsNoTracking()
                .ForCurrentUser(current)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

            return session is null ? Results.NotFound() : Results.Ok(session);
        });

        sessions.MapPut("/{id:guid}", async (Guid id, UpdateSessionRequest req, ICurrentUser current,
            TrainingDbContext db, CancellationToken ct) =>
        {
            var session = await db.WorkoutSessions
                .ForCurrentUser(current)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

            if (session is null) return Results.NotFound();

            session.Status = req.Status;
            session.CompletedAt = req.CompletedAt;
            session.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return Results.Ok(session);
        });

        sessions.MapDelete("/{id:guid}", async (Guid id, ICurrentUser current,
            TrainingDbContext db, CancellationToken ct) =>
        {
            var session = await db.WorkoutSessions
                .ForCurrentUser(current)
                .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

            if (session is null) return Results.NotFound();

            session.IsDeleted = true;
            session.DeletedAt = DateTime.UtcNow;
            session.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return Results.NoContent();
        });

        return app;
    }
}
