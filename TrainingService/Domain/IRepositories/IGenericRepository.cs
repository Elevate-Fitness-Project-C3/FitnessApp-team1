using TrainingService.Domain.Common;

namespace TrainingService.Domain.IRepositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void SaveInclude(T entity, params string[] includedProperties);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
