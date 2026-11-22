namespace TrainingService.Domain.Common;

/// <summary>Implemented by entities that contain personal data and must be scoped to one user.</summary>
public interface IUserOwned
{
    Guid UserId { get; }
}
