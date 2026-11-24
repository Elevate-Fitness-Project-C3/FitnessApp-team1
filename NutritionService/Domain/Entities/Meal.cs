using NutritionService.Domain.Enums;

namespace NutritionService.Domain.Entities;

public class Meal
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public MealType Type { get; private set; }
    public int Calories { get; private set; }
    public double ProteinGrams { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    // EF Core parameterless constructor
    private Meal() { }

    private Meal(Guid id, string name, MealType type, int calories, double proteinGrams, bool isActive, DateTime createdAtUtc, DateTime? updatedAtUtc)
    {
        Id = id;
        Name = name;
        Type = type;
        Calories = calories;
        ProteinGrams = proteinGrams;
        IsActive = isActive;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
    }

    public static Meal Create(string name, MealType type, int calories, double proteinGrams, DateTime utcNow)
    {
        ValidateInputs(name, type, calories, proteinGrams);

        return new Meal(
            Guid.NewGuid(),
            name.Trim(),
            type,
            calories,
            proteinGrams,
            isActive: true,
            createdAtUtc: utcNow,
            updatedAtUtc: null
        );
    }

    public void UpdateDetails(string name, MealType type, int calories, double proteinGrams, DateTime utcNow)
    {
        ValidateInputs(name, type, calories, proteinGrams);

        Name = name.Trim();
        Type = type;
        Calories = calories;
        ProteinGrams = proteinGrams;
        UpdatedAtUtc = utcNow;
    }

    public void Deactivate(DateTime utcNow)
    {
        IsActive = false;
        UpdatedAtUtc = utcNow;
    }

    private static void ValidateInputs(string name, MealType type, int calories, double proteinGrams)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (name.Trim().Length > 200)
        {
            throw new ArgumentException("Name cannot exceed 200 characters.", nameof(name));
        }

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentException("Invalid meal type.", nameof(type));
        }

        if (calories < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(calories), "Calories must be non-negative.");
        }

        if (proteinGrams < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(proteinGrams), "Protein must be non-negative.");
        }
    }
}
