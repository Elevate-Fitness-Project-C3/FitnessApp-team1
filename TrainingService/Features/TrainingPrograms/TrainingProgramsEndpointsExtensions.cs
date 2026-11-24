using TrainingService.Features.TrainingPrograms.ArchiveTrainingProgram.Controllers;
using TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Controllers;

namespace TrainingService.Features.TrainingPrograms
{
    public static class TrainingProgramsEndpointsExtensions
    {
        public static IEndpointRouteBuilder MapTrainingProgramsEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapArchiveTrainingProgramEndpoint();
            app.MapPublishTrainingProgramEndpoint();
            return app;
        }
    }
}
