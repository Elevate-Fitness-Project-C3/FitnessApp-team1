using Microsoft.EntityFrameworkCore;
using TrainingService.Extensions;
using TrainingService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSqlServerConfiguration(builder.Configuration);   
builder.Services.AddMediatRConfiguration();               


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    // Dev only: apply migrations automatically so docker compose up creates TrainingDb.
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<TrainingDbContext>().Database.Migrate();
}

app.UseHttpsRedirection();



app.Run();

