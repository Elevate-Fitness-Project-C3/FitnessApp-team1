using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.ProgramWorkoutSlotFeature.Endpoints;
using TrainingService.Features.TrainingProgramFeature.Endpoints;
using TrainingService.Infrastructure.DataAccess;
using TrainingService.Infrastructure.Persistence;

namespace TrainingService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
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

        public static IServiceCollection AddMediatRConfiguration(this IServiceCollection services)
        {
            services.AddMediatR(typeof(ServiceCollectionExtensions).Assembly);
            return services;
        }
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            return services;
        }
        public static IEndpointRouteBuilder MapWorkoutsEndpoints(this IEndpointRouteBuilder app)
        {
            //app.MapCreateWorkoutEndpoint();
            //app.MapGetWorkouts();      
            //app.MapGetWorkout();        
            //app.MapUpdateWorkoutEndpoint();
            //app.MapDeleteWorkoutEndpoint();

            return app;
        }
        //public static IEndpointRouteBuilder MapExercisesEndpoints(this IEndpointRouteBuilder app)
        //{
        //    app.MapCreateExerciseEndpoint();
        //    app.MapBrowseExercisesEndpoint();
        //    app.MapGetExerciseByIdEndpoint();
        //    app.MapUpdateExerciseDescriptionEndpoint();
        //    app.MapDeactivateExerciseEndpoint();
        //    return app;
        //}

        public static IEndpointRouteBuilder MapTrainingProgramEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapBrowsePublishedProgramsEndpoint();
            app.MapCreateTrainingProgramEndpoint();
            return app;
        }
        public static IEndpointRouteBuilder MapProgramWorkoutSlotEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapCreateProgramWorkoutSlotEndpoint();
            return app;
        }
    }
}