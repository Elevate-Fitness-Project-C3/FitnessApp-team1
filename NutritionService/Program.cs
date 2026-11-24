using System.Text.Json.Serialization;
using FluentValidation;
using FitnessApp.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NutritionService.API.Endpoints;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Application.Common.Options;
using NutritionService.Infrastructure.Clients;
using NutritionService.Infrastructure.Persistence;
using NutritionService.Infrastructure.Persistence.Repositories;
using NutritionService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization (support string enum representations)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configure Database
if (!builder.Environment.IsEnvironment("Testing"))
{
    var connectionString = builder.Configuration.GetConnectionString("Elevate_NutritionDb")
        ?? throw new InvalidOperationException("Connection string 'Elevate_NutritionDb' was not found.");

    builder.Services.AddDbContext<NutritionDbContext>(options =>
        options.UseSqlServer(connectionString));
}

// Add Repository & Unit of Work
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add FitnessApp.Auth and CurrentUserService
builder.Services.AddFitnessAuth(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Configure CalorieTarget and FitnessProfile Options
builder.Services.Configure<CalorieTargetOptions>(
    builder.Configuration.GetSection(CalorieTargetOptions.SectionName));
builder.Services.Configure<FitnessProfileOptions>(
    builder.Configuration.GetSection(FitnessProfileOptions.SectionName));

// Add Typed HttpClient for FitnessProfileClient
builder.Services.AddHttpClient<IFitnessProfileClient, FitnessProfileClient>((serviceProvider, client) =>
{
    var profileOptions = serviceProvider.GetRequiredService<IOptions<FitnessProfileOptions>>().Value;
    client.BaseAddress = new Uri(profileOptions.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(profileOptions.TimeoutSeconds);
});

// Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add OpenAPI / Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map Nutrition Minimal API Endpoints
app.MapNutritionEndpoints();
app.MapMealEndpoints();
app.MapCalorieTargetEndpoints();

app.Run();

public partial class Program { }
