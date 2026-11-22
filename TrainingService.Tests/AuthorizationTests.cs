using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FitnessApp.Auth;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using Xunit;

namespace TrainingService.Tests;

public sealed class AuthorizationTests : IDisposable
{
    private readonly TrainingWebApplicationFactory _factory = new();
    private readonly HttpClient _anonymous;

    public AuthorizationTests()
    {
        _anonymous = _factory.CreateClient();
        _factory.ResetDatabase();
    }

    [Fact]
    public async Task NoToken_Returns401()
    {
        var response = await _anonymous.GetAsync("/api/authz/admin-ping");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task NoToken_OnPersonalData_Returns401()
    {
        var response = await _anonymous.GetAsync("/api/dev/sessions");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task InvalidSignature_Returns401()
    {
        var token = TestTokens.Create(Guid.NewGuid(), Roles.Admin,
            signingKey: "a-different-signing-key-that-is-also-long-enough-987654321");
        var client = Client(token);

        var response = await client.GetAsync("/api/authz/admin-ping");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ExpiredToken_Returns401()
    {
        var token = TestTokens.Create(Guid.NewGuid(), Roles.Admin, DateTime.UtcNow.AddMinutes(-1));
        var client = Client(token);

        var response = await client.GetAsync("/api/authz/admin-ping");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserToken_OnAdminMutation_Returns403_AndChangesNothing()
    {
        var before = ExerciseCount();
        var client = Client(TestTokens.Create(Guid.NewGuid(), Roles.User));

        var response = await client.PostAsJsonAsync("/api/dev/catalog/exercises", new { name = "Should not exist" });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(before, ExerciseCount());
    }

    [Fact]
    public async Task UserToken_CanReadCatalog()
    {
        var client = Client(TestTokens.Create(Guid.NewGuid(), Roles.User));

        var response = await client.GetAsync("/api/dev/catalog/exercises");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task AdminToken_OnAdminMutation_Succeeds()
    {
        var before = ExerciseCount();
        var client = Client(TestTokens.Create(Guid.NewGuid(), Roles.Admin));

        var response = await client.PostAsJsonAsync("/api/dev/catalog/exercises", new { name = "Admin created" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(before + 1, ExerciseCount());
    }

    [Fact]
    public async Task TokenWithoutRole_Returns403_OnAdminEndpoint()
    {
        var client = Client(TestTokens.Create(Guid.NewGuid(), role: null));

        var response = await client.GetAsync("/api/authz/admin-ping");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UserA_CannotReadUpdateOrDeleteUserBSession_AndDataIsUnchanged()
    {
        var userA = Guid.NewGuid();
        var own = NewSession(userA);
        var other = NewSession(Guid.NewGuid());
        _factory.Seed(own, other);

        var client = Client(TestTokens.Create(userA, Roles.User));
        var before = Snapshot(other.Id);

        var get = await client.GetAsync($"/api/dev/sessions/{other.Id}");
        var put = await client.PutAsJsonAsync($"/api/dev/sessions/{other.Id}",
            new { status = (int)SessionStatus.Completed, completedAt = (DateTime?)DateTime.UtcNow });
        var delete = await client.DeleteAsync($"/api/dev/sessions/{other.Id}");

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, delete.StatusCode);
        Assert.Equal(before, Snapshot(other.Id));
    }

    [Fact]
    public async Task UserA_CanReadAndUpdateOwnSession()
    {
        var userA = Guid.NewGuid();
        var own = NewSession(userA);
        var other = NewSession(Guid.NewGuid());
        _factory.Seed(own, other);

        var client = Client(TestTokens.Create(userA, Roles.User));

        var list = await client.GetAsync("/api/dev/sessions");
        var get = await client.GetAsync($"/api/dev/sessions/{own.Id}");
        var put = await client.PutAsJsonAsync($"/api/dev/sessions/{own.Id}",
            new { status = (int)SessionStatus.Completed, completedAt = (DateTime?)DateTime.UtcNow });

        Assert.Equal(HttpStatusCode.OK, list.StatusCode);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
        Assert.Equal(HttpStatusCode.OK, put.StatusCode);

        using var db = _factory.CreateContext();
        var updated = await db.WorkoutSessions.AsNoTracking().SingleAsync(x => x.Id == own.Id);
        Assert.Equal(SessionStatus.Completed, updated.Status);
        Assert.NotNull(updated.CompletedAt);
        Assert.False(updated.IsDeleted);
    }

    private HttpClient Client(string token)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private int ExerciseCount()
    {
        using var db = _factory.CreateContext();
        return db.Exercises.Count();
    }

    private (SessionStatus Status, DateTime? CompletedAt, bool IsDeleted, DateTime? UpdatedAt) Snapshot(Guid id)
    {
        using var db = _factory.CreateContext();
        var s = db.WorkoutSessions.AsNoTracking().Single(x => x.Id == id);
        return (s.Status, s.CompletedAt, s.IsDeleted, s.UpdatedAt);
    }

    private static WorkoutSession NewSession(Guid userId) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        UserTrainingProgramId = Guid.NewGuid(),
        ProgramWorkoutSlotId = Guid.NewGuid(),
        Status = SessionStatus.InProgress,
        StartedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
    };

    public void Dispose()
    {
        _factory.Dispose();
    }
}
