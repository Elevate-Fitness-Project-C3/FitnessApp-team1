using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NutritionService.Application.Common.Interfaces;

namespace NutritionService.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly NutritionDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(NutritionDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> GetTableNoTracking()
    {
        return _dbSet.AsNoTracking();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        return await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;
        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }
        return await query.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public void SaveInclude(T entity, params string[] includedProperties)
    {
        var entry = _context.Entry(entity);
        foreach (var property in includedProperties)
        {
            entry.Property(property).IsModified = true;
        }
    }
}
