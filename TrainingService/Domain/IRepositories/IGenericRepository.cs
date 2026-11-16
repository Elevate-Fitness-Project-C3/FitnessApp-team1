
using System.Linq.Expressions;
using TrainingService.Domain.Common;

namespace TrainingService.Domain.IRepositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        //IQueryable<T> GetAllFilter(bool includeDeleted = false);

        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveInclude(T entity, params string[] includedProperties);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
        Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
