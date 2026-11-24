namespace NutritionService.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid GetRequiredUserId();
    bool IsAdmin { get; }
}
