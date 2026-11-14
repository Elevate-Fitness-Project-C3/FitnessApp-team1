using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TrainingService.Domain.Common;
using TrainingService.Domain.IRepositories;
using TrainingService.Infrastructure.Persistence;

namespace TrainingService.Infrastructure.DataAccess
{
    public class GenericRepository<T>:IGenericRepository<T> where T : BaseEntity
    {
        private readonly TrainingDbContext _context;
        public GenericRepository(TrainingDbContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

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

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }


    }
}
