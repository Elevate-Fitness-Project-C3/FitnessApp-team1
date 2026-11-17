using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TrainingService.Domain.Common;
using TrainingService.Domain.IRepositories;
using TrainingService.Infrastructure.Persistence;

namespace TrainingService.Infrastructure.DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly TrainingDbContext _context;
        public GenericRepository(TrainingDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
            => _context.Set<T>();
        
        public IQueryable<T> GetAllIncludingDeleted()
            => _context.Set<T>().IgnoreQueryFilters().AsNoTracking();
        public async Task<IEnumerable<T>> GetAllAsync()
            => await _context.Set<T>().ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id)
            => await _context.Set<T>().FindAsync(id);

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void SaveInclude(T entity, params string[] includedProperties)
        {
            // 1. Check if the entity is already being tracked in m emory
            var localEntity = _context.Set<T>().Local.FirstOrDefault(e => e.Id == entity.Id);
            EntityEntry entry;

            if (localEntity == null)
            {
                entry = _context.Attach(entity);
            }
            else
            {
                entry = _context.Entry(localEntity);
                entry.CurrentValues.SetValues(entity);
            }

            // 3. Loop through all properties of the entity
            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsPrimaryKey())
                    continue;

                property.IsModified = includedProperties.Contains(property.Metadata.Name);
            }

        }


        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
            => _context.Set<T>().AnyAsync(predicate, cancellationToken);

        public Task<int> CountAsync(Expression<Func<T, bool>>? criteria = null)
            => criteria is null
                ? _context.Set<T>().CountAsync()
                : _context.Set<T>().CountAsync(criteria);
        public Task DeleteAsync(Guid id, CancellationToken cancellationToken)
            => UpdateAsync(
                e => e.Id == id,
                setters => setters
                    .SetProperty(e => e.IsDeleted, true)
                    .SetProperty(e => e.DeletedAt, DateTime.UtcNow),
                cancellationToken);

        public Task UpdateAsync(
            Expression<Func<T, bool>> predicate,
            Action<UpdateSettersBuilder<T>> setters,
            CancellationToken cancellationToken)
            => _context.Set<T>()
                .Where(predicate)
                .ExecuteUpdateAsync(setters, cancellationToken);

      
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }


    }
}