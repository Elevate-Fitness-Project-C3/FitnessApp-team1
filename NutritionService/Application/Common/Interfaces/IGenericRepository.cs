using System.Linq.Expressions;

namespace NutritionService.Application.Common.Interfaces;

public interface IGenericRepository<T> where T : class
{
    IQueryable<T> GetTableNoTracking();
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
    void SaveInclude(T entity, params string[] includedProperties);
}
