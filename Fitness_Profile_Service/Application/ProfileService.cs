using FitnessProfileService.Domain;
using FitnessProfileService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitnessProfileService.Application;

public sealed class ProfileService : IOnboardingGate
{
    private readonly FitnessProfileDbContext _db;
    private readonly TimeProvider _clock;

    public ProfileService(FitnessProfileDbContext db, TimeProvider clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<ProfileChangeResult> OnboardAsync(
        Guid userId,
        OnboardingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await _db.FitnessProfiles.AnyAsync(p => p.UserId == userId, cancellationToken))
            return ProfileChangeResult.Conflict();

        var nowUtc = _clock.GetUtcNow().UtcDateTime;
        var profile = FitnessProfile.Create(
            userId,
            request.Gender,
            request.DateOfBirth,
            request.FitnessGoal,
            request.PhysicalActivityLevel,
            nowUtc,
            request.TrainingExperienceLevel,
            request.AvailableDaysPerWeek);

        var measurement = BodyMeasurement.Create(userId, request.HeightCm, request.WeightKg, nowUtc);
        profile.RefreshOnboardingStatus(true, nowUtc);

        _db.FitnessProfiles.Add(profile);
        _db.BodyMeasurements.Add(measurement);
        await _db.SaveChangesAsync(cancellationToken);

        return ProfileChangeResult.Created(MapProfile(profile, measurement));
    }

    public async Task<ProfileResponse?> GetProfileAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await _db.FitnessProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted, cancellationToken);

        if (profile is null)
            return null;

        var latest = await LatestMeasurementAsync(userId, cancellationToken);
        return MapProfile(profile, latest);
    }

    public async Task<IReadOnlyList<BodyMeasurementResponse>> GetMeasurementsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var items = await _db.BodyMeasurements
            .AsNoTracking()
            .Where(m => m.UserId == userId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.MeasurementId)
            .Select(m => new BodyMeasurementResponse(
                m.MeasurementId,
                m.Height,
                m.Weight,
                m.CreatedAt))
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<BodyMeasurementResponse> AddMeasurementAsync(
        Guid userId,
        MeasurementRequest request,
        CancellationToken cancellationToken = default)
    {
        var measurement = BodyMeasurement.Create(userId, request.HeightCm, request.WeightKg, _clock.GetUtcNow().UtcDateTime);
        _db.BodyMeasurements.Add(measurement);
        await _db.SaveChangesAsync(cancellationToken);

        return new BodyMeasurementResponse(
            measurement.MeasurementId,
            measurement.Height,
            measurement.Weight,
            measurement.CreatedAt);
    }

    public async Task<bool> IsCompletedAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _db.FitnessProfiles
            .AsNoTracking()
            .AnyAsync(p => p.UserId == userId && !p.IsDeleted && p.IsOnboardingCompleted, cancellationToken);

    private async Task<BodyMeasurement?> LatestMeasurementAsync(
        Guid userId,
        CancellationToken cancellationToken)
        => await _db.BodyMeasurements
            .AsNoTracking()
            .Where(m => m.UserId == userId && !m.IsDeleted)
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.MeasurementId)
            .FirstOrDefaultAsync(cancellationToken);

    private ProfileResponse MapProfile(FitnessProfile profile, BodyMeasurement? latest)
    {
        var age = AgeCalculator.AgeOn(profile.DateOfBirth, DateOnly.FromDateTime(_clock.GetUtcNow().UtcDateTime));

        return new ProfileResponse(
            profile.UserId,
            age,
            profile.Gender,
            profile.DateOfBirth,
            profile.PrimaryFitnessGoal,
            profile.PhysicalActivityLevel,
            profile.TrainingExperienceLevel,
            profile.AvailableDaysPerWeek,
            profile.IsOnboardingCompleted,
            latest is null
                ? null
                : new BodyMeasurementResponse(
                    latest.MeasurementId,
                    latest.Height,
                    latest.Weight,
                    latest.CreatedAt));
    }
}
