using MediatR;
using Microsoft.EntityFrameworkCore;
using TrainingService.Domain.Entities;
using TrainingService.Domain.Enums;
using TrainingService.Domain.IRepositories;
using TrainingService.Features.Shared;
using TrainingService.Features.TrainingPrograms.Common.Clients.FitnessProfile;
using TrainingService.Features.TrainingPrograms.Common.GetProgramScoringDetails.Queries;
using TrainingService.Features.TrainingPrograms.Common.Rules;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Dtos;
using TrainingService.Features.TrainingPrograms.ProgramRecommendations.Queries;

namespace TrainingService.Features.TrainingPrograms.ProgramRecommendations.Handlers;

public class GetProgramRecommendationsHandler: IRequestHandler<GetProgramRecommendationsQuery, RequestResponse<List<ProgramRecommendationDto>>>
{
    private readonly IGenericRepository<TrainingProgram> _programRepo;
    private readonly IFitnessProfileClient _profileClient;
    private readonly IMediator _mediator;

    public GetProgramRecommendationsHandler(
        IGenericRepository<TrainingProgram> programRepo,
        IFitnessProfileClient profileClient,
        IMediator mediator) => (_programRepo, _profileClient, _mediator) = (programRepo, profileClient, mediator);

    public async Task<RequestResponse<List<ProgramRecommendationDto>>> Handle(GetProgramRecommendationsQuery request, CancellationToken cancellationToken)
    {
        UserProfileDto profile;
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
        var programsResult = await _mediator.Send(new GetPublishedProgramsScoringDetailsQuery(), cancellationToken);

        if (!programsResult.Success)
        {
            return RequestResponse<List<ProgramRecommendationDto>>.Fail(
                programsResult.Message, programsResult.StatusCode);
        }

        var recommendations = programsResult.Data
             .Select(dto =>
             {
                 // Manual Mapping
                 var programEntity = new TrainingProgram
                 {
                     Id = dto.ProgramId,
                     Title = dto.Title,
                     Goal = dto.Goal,
                     RequiredLevel = dto.Level,   
                     DaysPerWeek = dto.DaysPerWeek,
                     Status = dto.Status,
                     AccessTier = dto.AccessTier
                 };

                 var (score, warnings) = ProgramRecommendationRules.Calculate(programEntity, profile);

                 return new ProgramRecommendationDto(dto.ProgramId, dto.Title, score, warnings);
             })
             .OrderByDescending(r => r.MatchScore)
             .ThenBy(r => r.Warnings.Count)
             .ThenBy(r => r.Title)
             .ToList();

        return RequestResponse<List<ProgramRecommendationDto>>.Ok(recommendations);
    }
}