using FitnessApp.Auth;
using Microsoft.EntityFrameworkCore;
using TrainingService.Extensions;
using TrainingService.Features.Authz;
using TrainingService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

// WebApplicationFactory uses the Testing environment and supplies SQLite instead.
if (!builder.Environment.IsEnvironment("Testing"))
    builder.Services.AddSqlServerConfiguration(builder.Configuration);

builder.Services.AddMediatRConfiguration();
builder.Services.AddFitnessAuth(builder.Configuration);

var app = builder.Build();

// Auth-only endpoints are mapped in Development and in the "Testing" environment used by
// WebApplicationFactory. Production has no dev endpoints; OpenAPI stays anonymous in Development.
var exposeAuthEndpoints = app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();

    // Dev only: apply migrations automatically so docker compose up creates TrainingDb.
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<TrainingDbContext>().Database.Migrate();
}

if (!exposeAuthEndpoints)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

if (exposeAuthEndpoints)
{
    app.MapAuthzDevEndpoints();
}

app.Run();

public partial class Program;
