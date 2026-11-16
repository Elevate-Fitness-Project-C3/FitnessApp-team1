using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using NutritionService.API.Endpoints;
using NutritionService.Application.Common.Interfaces;
using NutritionService.Infrastructure.Persistence;
using NutritionService.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON serialization (support string enum representations)
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("Elevate_NutritionDb")
    ?? throw new InvalidOperationException("Connection string 'Elevate_NutritionDb' was not found.");

builder.Services.AddDbContext<NutritionDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add HttpContextAccessor and CurrentUserService
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Add MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add Authentication & Authorization
builder.Services.AddAuthentication()
    .AddJwtBearer();
builder.Services.AddAuthorization();

// Add OpenAPI / Swagger
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Map Nutrition Minimal API Endpoints
app.MapNutritionEndpoints();

app.Run();
