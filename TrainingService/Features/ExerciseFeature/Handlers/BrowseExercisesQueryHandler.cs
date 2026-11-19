using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.DTOs;
using TrainingService.Features.ExerciseFeature.Queries;
using TrainingService.Features.Shared;
using TrainingService.Infrastructure.Persistence;
using TrainingService.Mapping;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace TrainingService.Features.ExerciseFeature.Handlers
{
    public class BrowseExercisesQueryHandlerL : IRequestHandler<BrowseExercisesQuery, RequestResponse<PaginatedResult<ExerciseDto>>>
    {
        private readonly IGenericRepository<Exercise> _exerciseRepository;
        public BrowseExercisesQueryHandlerL(IGenericRepository<Exercise> exerciseRepository, TrainingDbContext context)
        {
            _exerciseRepository = exerciseRepository;
        }
        public async Task<RequestResponse<PaginatedResult<ExerciseDto>>> Handle(
    BrowseExercisesQuery request,
    CancellationToken ct)
        {
            var query = _exerciseRepository.GetAll(); 

            //Apply name filter only when a name is provided.
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                query = query.Where(e =>
                    e.Name.Contains(request.Name.Trim()));
            }

            // Apply target muscle filter only when provided.
            if (!string.IsNullOrWhiteSpace(request.TargetMuscle))
            {
                if (!Enum.TryParse<MuscleGroup>(
                        request.TargetMuscle, true, out var targetMuscle)
                    || !Enum.IsDefined(typeof(MuscleGroup), targetMuscle))
                {
                    return RequestResponse<PaginatedResult<ExerciseDto>>
                        .Fail("Invalid target muscle.", 400);
                }

                query = query.Where(e => e.TargetMuscle == targetMuscle);
            }

            // Apply tracking type filter only when provided.
            if (!string.IsNullOrWhiteSpace(request.TrackingType))
            {
                if (!Enum.TryParse<TrackingType>(
                        request.TrackingType, true, out var trackingType)
                    || !Enum.IsDefined(typeof(TrackingType), trackingType))
                {
                    return RequestResponse<PaginatedResult<ExerciseDto>>
                        .Fail("Invalid tracking type.", 400);
                }

                query = query.Where(e => e.TrackingType == trackingType);
            }

            // Get total count before applying pagination.
            var totalCount = await query.CountAsync();

            // Normalize pagination values.
            var pageIndex = Math.Max(1, request.PageNumber);
            var pageSize = request.PageSize < 1 || request.PageSize > 100
                ? 10
                : request.PageSize;

            // Apply pagination and retrieve only the requested page.
             var exerciseDtos = await query
                .OrderBy(e => e.Name)
                .ThenBy(e=>e.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(e=>e.ToExerciseDto())
                //.Select(e => new ExerciseDto
                //{
                //    Id = e.Id,
                //    Name = e.Name,
                //    Description = e.Description,
                //    TargetMuscle = e.TargetMuscle,
                //    TrackingType = e.TrackingType
                //})
                .ToListAsync(ct);

            var result = new PaginatedResult<ExerciseDto>(
                exerciseDtos,
                totalCount,
                pageIndex,
                pageSize);

            return RequestResponse<PaginatedResult<ExerciseDto>>.Ok(result);
        }
    }
}
