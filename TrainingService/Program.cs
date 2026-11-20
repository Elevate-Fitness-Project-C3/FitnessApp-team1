using TrainingService.Extensions;
using TrainingService.Features.ProgramWorkoutSlotFeature.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSqlServerConfiguration(builder.Configuration);
builder.Services.AddInfrastructureServices();
builder.Services.AddMediatRConfiguration();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //app.MapScalarApiReference();
}

app.UseHttpsRedirection();
//app.MapExercisesEndpoints();
app.MapTrainingProgramEndpoints();
app.MapCreateProgramWorkoutSlotEndpoint();

app.Run();