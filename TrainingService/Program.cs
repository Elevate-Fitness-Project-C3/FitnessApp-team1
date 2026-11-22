using Microsoft.EntityFrameworkCore;
using TrainingService.Infrastructure.Persistence;
using TrainingService.Extensions;
using TrainingService.Features.TrainingPrograms;
using TrainingService.Features.WorkoutExercises;
using TrainingService.Features.Workouts;
using TrainingService.Features.ProgramWorkoutSlotFeature.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddSqlServerConfiguration(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddMediatRConfiguration();

// WebApplicationFactory uses the Testing environment and supplies SQLite instead.
if (!builder.Environment.IsEnvironment("Testing"))
    builder.Services.AddSqlServerConfiguration(builder.Configuration);

builder.Services.AddMediatRConfiguration();
builder.Services.AddFitnessAuth(builder.Configuration);

var app = builder.Build();

// Local Docker development: run this service's own migrations before accepting requests.
// Keep this opt-in so deployment environments can manage schema changes separately.
if (app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
{
    await using var migrationScope = app.Services.CreateAsyncScope();
    var dbContext = migrationScope.ServiceProvider.GetRequiredService<TrainingDbContext>();

    app.Logger.LogInformation("Applying TrainingService database migrations...");
    await dbContext.Database.MigrateAsync();
    app.Logger.LogInformation("TrainingService database migrations applied.");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Fitness API v1");
    });
}
app.MapWorkoutsEndpoints();
app.MapWorkoutExerciseEndpoints();
app.MapTrainingProgramEndpoints();
app.MapCreateProgramWorkoutSlotEndpoint();


app.Run();
