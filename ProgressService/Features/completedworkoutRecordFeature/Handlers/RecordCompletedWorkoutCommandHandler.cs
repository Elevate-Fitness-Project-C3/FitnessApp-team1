using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProgressService.Domain.Entities;
using ProgressService.Domain.IRepositories;
using ProgressService.Features.completedworkoutRecordFeature.Commands;

namespace ProgressService.Features.completedworkoutRecordFeature.Handlers
{
    public class RecordCompletedWorkoutCommandHandler : IRequestHandler<RecordCompletedWorkoutCommand, bool>
    {
        private readonly IGenericRepository<CompletedWorkoutRecord> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RecordCompletedWorkoutCommandHandler(IGenericRepository<CompletedWorkoutRecord> repository,IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(RecordCompletedWorkoutCommand request, CancellationToken cancellationToken)
        {
            //1.Validation of Identifiers :
            if (request.UserId == Guid.Empty ||
                request.WorkoutSessionId == Guid.Empty ||
                request.TrainingProgramId == Guid.Empty ||
                request.ProgramWorkoutSlotId == Guid.Empty)
                throw new ArgumentException("Event contains an empty identifier.");

            //2. Validation of Completion Time:
            if (request.CompletedAtUtc == default || request.CompletedAtUtc > DateTime.UtcNow.AddMinutes(5))
                throw new ArgumentException("Invalid completion time.");

            //3.Create a new CompletedWorkoutRecord entity:
            var record = new CompletedWorkoutRecord
            {
                UserId = request.UserId,
                WorkoutSessionId = request.WorkoutSessionId,
                TrainingProgramId = request.TrainingProgramId,
                ProgramWorkoutSlotId = request.ProgramWorkoutSlotId,
                CompletedAtUtc = request.CompletedAtUtc
            };

            //4. Save the entity to the database using the repository and unit of work:
            try
            {
                await _unitOfWork.ExecuteAsync(() =>
                {
                    _repository.Add(record);
                    return Task.CompletedTask;
                }, cancellationToken);

                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                //The record already exists" and not a failure :
                return false;
            }
        }
    }
}
