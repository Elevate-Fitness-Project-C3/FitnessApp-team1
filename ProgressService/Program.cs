using FitnessApp.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProgressService.Domain.IRepositories;
using ProgressService.Features.WeeklyStreakFeature.EndPoints;
using ProgressService.Features.WorkoutHistoryFeature.EndPoints;
using ProgressService.Features.WorkoutStatisticsFeature.EndPoints;
using ProgressService.Infrastrucure.DataAccess;
using ProgressService.Infrastrucure.Persistence.Configurations;
using ProgressService.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ProgressDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddMediatR(typeof(Program).Assembly);
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddSingleton(TimeProvider.System);

//add authentication and authorization services
builder.Services.AddFitnessAuth(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapGetWorkoutHistory();
app.MapGetProgressSummary();
app.MapGetWeeklyStreak();

app.Run();
