using Elevate.Contracts;
using MassTransit;
using MediatR;
using ProgressService.Features.completedworkoutRecordFeature.Commands;

namespace ProgressService.Features.completedworkoutRecordFeature.Consumers
{
    public class WorkoutCompletedConsumer : IConsumer<WorkoutCompletedMessage>
    {
        private readonly IMediator _mediator;

        public WorkoutCompletedConsumer(IMediator mediator) => _mediator = mediator;
        public async Task Consume(ConsumeContext<WorkoutCompletedMessage> context)
        {
            var message = context.Message;
            await _mediator.Send(new RecordCompletedWorkoutCommand(
                message.UserId,
                message.WorkoutSessionId,
                message.TrainingProgramId,
                message.ProgramWorkoutSlotId,
                message.CompletedAtUtc),
                context.CancellationToken);
        }
    }
}
