using NutritionService.Domain.Enums;

namespace NutritionService.Domain.Entities;

public class NutritionPreference
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public NutritionGoal Goal { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    // EF Core parameterless constructor
    private NutritionPreference() { }

    public NutritionPreference(Guid id, Guid userId, NutritionGoal goal, DateTime utcNow)
    {
        Id = id;
        UserId = userId;
        Goal = goal;
        CreatedAtUtc = utcNow;
        UpdatedAtUtc = utcNow;
    }

    public static NutritionPreference Create(Guid userId, NutritionGoal goal, DateTime utcNow)
    {
        return new NutritionPreference(Guid.NewGuid(), userId, goal, utcNow);
    }

    public void UpdateGoal(NutritionGoal newGoal, DateTime utcNow)
    {
        Goal = newGoal;
        UpdatedAtUtc = utcNow;
    }
}
