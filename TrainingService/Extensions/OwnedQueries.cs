using FitnessApp.Auth;
using TrainingService.Domain.Common;

namespace TrainingService.Extensions;

/// <summary>
/// Reusable ownership filter for personal data. All queries for IUserOwned entities must
/// use this helper with ICurrentUser; never trust a userId from a route, query or body.
/// </summary>
public static class OwnedQueries
{
    public static IQueryable<T> ForCurrentUser<T>(this IQueryable<T> query, ICurrentUser currentUser)
        where T : class, IUserOwned
        => query.Where(x => x.UserId == currentUser.UserId);
}
