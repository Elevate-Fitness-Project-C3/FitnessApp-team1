namespace FitnessProfileService.Domain;

public sealed class BodyMeasurement : BaseEntity
{
    private BodyMeasurement() { }

    public long MeasurementId { get; private set; }
    public Guid UserId { get; private set; }
    public decimal Height { get; private set; }
    public decimal Weight { get; private set; }

    public static BodyMeasurement Create(Guid userId, decimal heightCm, decimal weightKg, DateTime nowUtc) => new()
    {
        Id = Guid.NewGuid(),
        CreatedAt = nowUtc,
        UserId = userId,
        Height = heightCm,
        Weight = weightKg
    };
}
