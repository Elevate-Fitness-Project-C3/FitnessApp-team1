using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Clients;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Queries;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Rules;

namespace TrainingService.Features.TrainingPrograms.ProgramRecommendations.Handlers;

public class GetProgramRecommendationsHandler: IRequestHandler<GetProgramRecommendationsQuery, RequestResponse<List<ProgramRecommendationDto>>>
{
    private readonly IGenericRepository<TrainingProgram> _programRepo;
    private readonly IFitnessProfileClient _profileClient;

    public GetProgramRecommendationsHandler(
        IGenericRepository<TrainingProgram> programRepo,
        IFitnessProfileClient profileClient) => (_programRepo, _profileClient) = (programRepo, profileClient);

    public async Task<RequestResponse<List<ProgramRecommendationDto>>> Handle(GetProgramRecommendationsQuery request, CancellationToken cancellationToken)
    {
        UserProfileDto? profile;
        try
        {
            profile = await _profileClient.GetProfileAsync(cancellationToken);
        }
        catch (UnauthorizedAccessException)
        {
            return RequestResponse<List<ProgramRecommendationDto>>.Fail("Unauthorized.", 401);
        }
        catch (Exception)
        {
            return RequestResponse<List<ProgramRecommendationDto>>.Fail("Recommendations are temporarily unavailable. Please try again later.", 503);
        }

        if (profile is null || !profile.IsOnboardingCompleted)
        {
            return RequestResponse<List<ProgramRecommendationDto>>.Fail( "Please complete your fitness profile to get personalized recommendations.", 400);
        }

        // get all published programs from the repository
        var publishedPrograms = await _programRepo
            .GetAll()
            .Where(p => p.Status == ProgramStatus.Published)
            .ToListAsync(cancellationToken);

        // first with the highest match score, then with the least number of warnings, and finally alphabetically by title
        var recommendations = publishedPrograms
            .Select(p =>
            {
                var (score, warnings) = ProgramRecommendationRules.Calculate(p, profile);
                return new ProgramRecommendationDto(p.Id, p.Title, score, warnings);
            })
            .OrderByDescending(r => r.MatchScore)   
            .ThenBy(r => r.Warnings.Count)          
            .ThenBy(r => r.Title)                   
            .ToList();

        return RequestResponse<List<ProgramRecommendationDto>>.Ok(recommendations);
    }
}