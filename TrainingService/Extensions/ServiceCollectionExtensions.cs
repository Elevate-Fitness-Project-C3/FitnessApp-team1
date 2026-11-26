using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ProgramWorkoutSlotFeature.Endpoints;
using TrainingService.Features.Shared.Validation;
using TrainingService.Features.TrainingProgramFeature.Endpoints;
using TrainingService.Features.TrainingPrograms.ArchiveTrainingProgram.Controllers;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Clients;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Endpoints;
using TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Controllers;
using TrainingService.Infrastructure.DataAccess;
using TrainingService.Infrastructure.Persistence;

namespace TrainingService.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' is missing.");

        services.AddDbContext<TrainingDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
                sql.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null)));

        return services;
    }

    public static IServiceCollection AddMediatRConfiguration(
        this IServiceCollection services)
    {
        services.AddMediatR(
            typeof(ServiceCollectionExtensions).Assembly);

        services.AddValidatorsFromAssembly(
            typeof(ServiceCollectionExtensions).Assembly);

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped(
            typeof(IGenericRepository<>),
            typeof(GenericRepository<>));

        return services;
    }

    public static IEndpointRouteBuilder MapTrainingProgramEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapBrowsePublishedProgramsEndpoint();
        app.MapCreateTrainingProgramEndpoint();

        //publish and archive endpoints 
        app.MapArchiveTrainingProgramEndpoint();
        app.MapPublishTrainingProgramEndpoint();
        //program recommendations endpoint
        app.MapGetProgramRecommendationsEndpoint();

        return app;
    }

    public static IEndpointRouteBuilder MapProgramWorkoutSlotEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapCreateProgramWorkoutSlotEndpoint();

        return app;
    }


    public static IServiceCollection AddFitnessProfileClient(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient<IFitnessProfileClient, FitnessProfileClient>(client =>
        {
            var baseUrl = configuration["FitnessProfileService:BaseUrl"]
                ?? throw new InvalidOperationException("FitnessProfileService:BaseUrl is missing.");

            client.BaseAddress = new Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(
                configuration.GetValue<int>("FitnessProfileService:TimeoutSeconds", 5));
        });

        return services;
    }
}
