
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TrainingService.Domain.Entities;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.SetTrainingRefrence.DTO;

namespace TrainingService.Features.SetTrainingRefrence.Command;

public record UpdateTrainingPreferencesCommand(
    TrainingPreferencesDto Preferences)
    : IRequest<UpdateTrainingPreferencesResult>;

public record UpdateTrainingPreferencesResult(
    bool Succeeded,
    int StatusCode,
    string Message);

public class UpdateTrainingPreferencesCommandHandler
    : IRequestHandler<
        UpdateTrainingPreferencesCommand,
        UpdateTrainingPreferencesResult>
{
    private readonly IGenericRepository<FitnessProfile> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UpdateTrainingPreferencesCommandHandler(
        IGenericRepository<FitnessProfile> repository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<UpdateTrainingPreferencesResult> Handle(
        UpdateTrainingPreferencesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?
            .User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return new UpdateTrainingPreferencesResult(
                false,
                StatusCodes.Status401Unauthorized,
                "Unauthorized.");
        }

        UpdateTrainingPreferencesResult? result = null;

        await _unitOfWork.ExecuteAsync(async ct =>
        {
            var profile = await _repository.GetAll()
                .FirstOrDefaultAsync(
                    p => p.UserId == userId,
                    ct);

            if (profile is null)
            {
                result = new UpdateTrainingPreferencesResult(
                    false,
                    StatusCodes.Status404NotFound,
                    "Fitness profile was not found.");

                return;
            }

            profile.TrainingExperienceLevel =
                request.Preferences.TrainingExperienceLevel;

            profile.AvailableDaysPerWeek =
                request.Preferences.AvailableDaysPerWeek;

            _repository.SaveInclude(
                profile,
                nameof(FitnessProfile.TrainingExperienceLevel),
                nameof(FitnessProfile.AvailableDaysPerWeek));

            result = new UpdateTrainingPreferencesResult(
                true,
                StatusCodes.Status200OK,
                "Training preferences updated successfully.");

        }, cancellationToken);

        return result!;
    }
}
