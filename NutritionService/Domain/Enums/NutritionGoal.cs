using System.Text.Json.Serialization;

namespace NutritionService.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NutritionGoal
{
    Maintain = 1,
    GradualLoss = 2,
    GradualGain = 3
}
