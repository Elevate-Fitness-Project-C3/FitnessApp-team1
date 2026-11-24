using MediatR;
using System.Security.AccessControl;
using TrainingService.Features.ProgramWorkoutSlotFeature.Common.Queries;
using TrainingService.Features.ProgramWorkoutSlotFeature.Create.Commands;
using TrainingService.Features.ProgramWorkoutSlotFeature.Create.Orchestrator;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingProgramFeature.GetTrainingProgramById.Queries;
using TrainingService.Features.WorkoutTemplateFeature.Common.Queries;

namespace TrainingService.Features.ProgramWorkoutSlotFeature.Create.Handlers
{
    public class CreateProgramWorkoutSlotOrchestratorHandler : IRequestHandler<CreateProgramWorkoutSlotOrchestrator, RequestResponse<Guid>>
    {
        private readonly IMediator _mediator;
        public CreateProgramWorkoutSlotOrchestratorHandler(IMediator mediator)
        => _mediator = mediator;

        public async Task<RequestResponse<Guid>> Handle(CreateProgramWorkoutSlotOrchestrator request, CancellationToken cancellationToken)
        {
            var trainingProgramDto = await _mediator.Send(new GetTrainingProgramByIdQuery(request.ProgramId), cancellationToken);

            if (trainingProgramDto.Data == null)
                return RequestResponse<Guid>.Fail(trainingProgramDto.Message, trainingProgramDto.StatusCode);

            if (request.WeekNumber < 1|| request.WeekNumber > trainingProgramDto.Data.DurationWeeks)
                return RequestResponse<Guid>.Fail($"Week number must be between 1 and {trainingProgramDto.Data.DurationWeeks}.", 400);

            if (request.DayNumber < 1 || request.DayNumber > trainingProgramDto.Data.DaysPerWeek)
                return RequestResponse<Guid>.Fail($"Day of week must be between 1 and {trainingProgramDto.Data.DaysPerWeek}.", 400);


            var workoutExists = await _mediator.Send(new WorkoutExistsQuery(request.WorkoutId), cancellationToken);
            if (!workoutExists.Data)
                return RequestResponse<Guid>.Fail("Workout not found", 404);


            var programWorkoutSlotExists = await _mediator.Send(
                new ProgramWorkoutSlotExistsQuery(
                    request.ProgramId,
                    request.WeekNumber,
                    request.DayNumber), cancellationToken);

            if (programWorkoutSlotExists.Data)
                return RequestResponse<Guid>.Fail("Program workout slot already exists for the given week and day.", 409);

            var command = await _mediator.Send(new CreateProgramWorkoutSlotCommand(
                request.ProgramId,
                request.WorkoutId,
                request.WeekNumber,
                request.DayNumber   
            ), cancellationToken);

            if (!command.Success)
                return RequestResponse<Guid>.Fail(command.Message, command.StatusCode);
            return RequestResponse<Guid>.Ok(command.Data);
        }
    }
}
