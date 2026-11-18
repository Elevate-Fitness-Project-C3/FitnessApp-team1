using MediatR;
using TrainingService.DTOs;
using TrainingService.Features.Shared;

namespace TrainingService.Features.ExerciseFeature.Queries
{
    public record GetExerciseByIdQuery(Guid Id) : IRequest<RequestResponse<ExerciseDto>>;
    
}
