using System.Text.Json.Serialization;
using FitnessProfileService.Api;
using FitnessProfileService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddFitnessProfileServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();

    // Dev only: apply migrations so a fresh local SQL container gets the schema.
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<FitnessProfileDbContext>().Database.Migrate();
}

// Keep local HTTP/Swagger/dev-token flows working, matching the other services.
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapProfileEndpoints();

app.Run();

public partial class Program;
