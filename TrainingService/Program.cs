
using Microsoft.EntityFrameworkCore;
using TrainingService.Extensions;
using TrainingService.Features.TrainingPrograms;
using TrainingService.Features.WorkoutExercises;
using TrainingService.Features.Workouts;
using TrainingService.Features.ProgramWorkoutSlotFeature.Endpoints;
using TrainingService.Infrastructure.Persistence;
using FitnessApp.Auth;
using TrainingService.Features.SetTrainingRefrence.EndPoint;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddSqlServerConfiguration(
        builder.Configuration);
}

builder.Services.AddInfrastructureServices();
builder.Services.AddMediatRConfiguration();
builder.Services.AddFitnessAuth(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing") &&
    app.Configuration.GetValue<bool>(
        "Database:ApplyMigrationsOnStartup"))
{
    await using var migrationScope =
        app.Services.CreateAsyncScope();

    var dbContext = migrationScope.ServiceProvider
        .GetRequiredService<TrainingDbContext>();

    app.Logger.LogInformation(
        "Applying TrainingService database migrations...");

    await dbContext.Database.MigrateAsync();

    app.Logger.LogInformation(
        "TrainingService database migrations applied.");
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Fitness API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapWorkoutsEndpoints();
app.MapWorkoutExerciseEndpoints();
app.MapTrainingProgramEndpoints();
app.MapTrainingProgramsEndpoints();
app.MapCreateProgramWorkoutSlotEndpoint();

app.MapTrainingPreferencesEndpoint();



app.Run();

public partial class Program;
