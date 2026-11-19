using FitnessApp.Auth;
using IdentityService.Api;
using IdentityService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEmailVerification(builder.Configuration);
builder.Services.AddFitnessAuth(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Dev only: apply migrations automatically so local runs "just work".
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<EmailVerificationDbContext>().Database.Migrate();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEmailVerificationEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapDevFakeRegistration();
    // TODO: remove when ID-03 login issues real tokens.
    app.MapDevTokenEndpoint();
}

app.Run();
