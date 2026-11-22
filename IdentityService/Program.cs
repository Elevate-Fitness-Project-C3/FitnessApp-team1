using FluentValidation;
using FitnessApp.Auth;
using IdentityService.Api;
using IdentityService.Identity.User_Login.Endpoints;
using IdentityService.Identity.User_Registration.Commands;
using IdentityService.Identity.User_Registration.EndPoint;
using IdentityService.Identity.User_Registration.Validation;
using IdentityService.Infrastructure.Persistence;
using IdentityService.Persistence;
using IdentityService.Persistence.Seeding;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddEmailVerification(builder.Configuration);
builder.Services.AddFitnessAuth(builder.Configuration);

builder.Services.AddRateLimiter(config =>
{
    config.AddFixedWindowLimiter("fixed", options =>
    {
        options.Window = TimeSpan.FromMinutes(10);
        options.PermitLimit = 5;
    });
});

builder.Services.AddValidatorsFromAssemblyContaining<RegisterViewModelValidation>();
builder.Services.AddMediatR(typeof(RegisterUserCommand).Assembly);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<RoleSeeder>();

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.AllowedForNewUsers = true;
    options.SignIn.RequireConfirmedEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<AppDbContext>();

    if (app.Configuration.GetValue<bool>(
        "Database:ApplyMigrationsOnStartup"))
    {
        //await dbContext.Database.MigrateAsync();
    }

    var roleSeeder = scope.ServiceProvider
        .GetRequiredService<RoleSeeder>();

    await roleSeeder.SeedRolesAsync();
}

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<EmailVerificationDbContext>().Database.Migrate();

    app.MapSwagger().AllowAnonymous();
    app.MapSwaggerUI().AllowAnonymous();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapEmailVerificationEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapDevFakeRegistration();
    app.MapDevTokenEndpoint();
}

await app.RegisterUser();
app.UserLogin();

app.Run();
