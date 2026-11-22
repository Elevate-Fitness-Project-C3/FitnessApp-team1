using MediatR;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Dtos;
using TrainingService.Features.TrainingPrograms.Common.GetProgramStructure.Queries;
using TrainingService.Features.TrainingPrograms.Common.IsProgramInDraftStatus.Queries;
using TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Commands;

namespace TrainingService.Features.TrainingPrograms.PublishTrainingProgram.Handlers
{
    public class PublishTrainingProgramCommandHandler : IRequestHandler<PublishTrainingProgramCommand, RequestResponse<Unit>>
    {
        private readonly IGenericRepository<TrainingProgram> _programRepo;
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public PublishTrainingProgramCommandHandler(IGenericRepository<TrainingProgram> programRepo,
        IMediator mediator,
        IUnitOfWork unitOfWork)
        {
            _programRepo = programRepo;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<RequestResponse<Unit>> Handle(PublishTrainingProgramCommand request, CancellationToken cancellationToken)
        {
            var checkStatus = await _mediator.Send(new IsProgramInDraftStatusQuery(request.ProgramId), cancellationToken);

            if (!checkStatus.Success)
                return RequestResponse<Unit>.Fail(checkStatus.Message, checkStatus.StatusCode);

            var structureResult = await _mediator.Send(new GetProgramStructureQuery(request.ProgramId), cancellationToken);

            if (!structureResult.Success || structureResult.Data == null)
                return RequestResponse<Unit>.Fail(structureResult.Message, structureResult.StatusCode);

            // 4. Validation
            var errors = new Dictionary<string, List<string>>();
            ValidateDetails(structureResult.Data, errors);
            ValidateSlots(structureResult.Data, errors);
            ValidateWorkouts(structureResult.Data, errors);

            if (errors.Any())
            {
                var errorsDict = errors.ToDictionary(k => k.Key, v => v.Value.ToArray());
                return RequestResponse<Unit>.Fail("Program is not ready for publishing.", 400, errorsDict);
            }

            var programStatusUpdate = new TrainingProgram
            {
                Id = request.ProgramId,
                Status = ProgramStatus.Published
            };

            _programRepo.SaveInclude(programStatusUpdate, nameof(TrainingProgram.Status));

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return RequestResponse<Unit>.Ok(Unit.Value, "Program published successfully.");
        }


        private void ValidateDetails(ProgramStructureDto data, Dictionary<string, List<string>> errors)
        {
            if (string.IsNullOrWhiteSpace(data.Title))
                AddError(errors, "Title", "Title is required.");

            if (data.DurationWeeks <= 0)
                AddError(errors, "DurationWeeks", "Duration must be greater than 0.");

            if (data.DaysPerWeek <= 0 || data.DaysPerWeek > 7)
                AddError(errors, "DaysPerWeek", "DaysPerWeek must be between 1 and 7.");
        }

        private void ValidateSlots(ProgramStructureDto data, Dictionary<string, List<string>> errors)
        {
            var expected = data.DurationWeeks * data.DaysPerWeek;

            if (data.Slots.Count != expected)
                AddError(errors, "Slots", $"Expected {expected} slots but found {data.Slots.Count}.");

            var existing = new HashSet<(int, int)>();
            foreach (var s in data.Slots)
            {
                if (!existing.Add((s.WeekNumber, s.DayNumber)))
                    AddError(errors, "DuplicateSlot", $"Duplicate slot Week {s.WeekNumber}, Day {s.DayNumber}.");
            }

            for (int w = 1; w <= data.DurationWeeks; w++)
                for (int d = 1; d <= data.DaysPerWeek; d++)
                    if (!existing.Contains((w, d)))
                        AddError(errors, "MissingSlot", $"Slot Week {w}, Day {d} is missing.");
        }

        private void ValidateWorkouts(ProgramStructureDto data, Dictionary<string, List<string>> errors)
        {
            foreach (var s in data.Slots)
            {
                if (!s.HasWorkout)
                    AddError(errors, $"Slot_{s.WeekNumber}_{s.DayNumber}", "Workout does not exist.");
                else if (s.ExercisesCount == 0)
                    AddError(errors, $"Slot_{s.WeekNumber}_{s.DayNumber}", "Workout has no exercises.");
            }
        }

        private void AddError(Dictionary<string, List<string>> errors, string key, string message)
        {
            if (!errors.ContainsKey(key)) errors[key] = new List<string>();
            errors[key].Add(message);
        }
    }
}
