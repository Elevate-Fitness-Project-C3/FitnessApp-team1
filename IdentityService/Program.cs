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

//Register Fluent Validation 
builder.Services.AddValidatorsFromAssemblyContaining<RegisterViewModelValidation>();
builder.Services.AddMediatR(typeof(RegisterUserCommand).Assembly);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

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

    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();

    //await RoleSeeder.SeedRolesAsync(roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Dev only: apply migrations automatically so local runs "just work".
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<EmailVerificationDbContext>().Database.Migrate();

    app.MapSwagger().AllowAnonymous();
    app.MapSwaggerUI().AllowAnonymous();
}

// Local dev runs use HTTP (Swagger/MailHog). The https launch profile advertises
// port 7127 but does not bind it without a dev certificate, so redirecting there
// breaks POSTs from Swagger UI. Only redirect outside Development.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.MapEmailVerificationEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapDevFakeRegistration();
    // TODO: remove when ID-03 login issues real tokens.
    app.MapDevTokenEndpoint();
}

app.Run();
