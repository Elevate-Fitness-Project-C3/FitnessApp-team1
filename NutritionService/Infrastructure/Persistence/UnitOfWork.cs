using Microsoft.EntityFrameworkCore.Storage;
using NutritionService.Application.Common.Interfaces;

namespace NutritionService.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork, IAsyncDisposable, IDisposable
{
    private readonly NutritionDbContext _context;
    private IDbContextTransaction? _transaction;
    private int _depth = 0;
    private bool _hasRolledBack = false;

    public UnitOfWork(NutritionDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_hasRolledBack)
        {
            throw new InvalidOperationException("Cannot begin transaction because an inner operation already rolled back.");
        }

        if (_depth == 0)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        _depth++;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_hasRolledBack || _transaction == null)
        {
            throw new InvalidOperationException("Cannot commit transaction because an inner operation failed and rolled back.");
        }

        _depth--;

        if (_depth == 0)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
                await _transaction.CommitAsync(cancellationToken);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(CancellationToken.None);
            }
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
            _hasRolledBack = true;
            _depth = 0;
        }
    }

    public async Task ExecuteAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            await action(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<CancellationToken, Task<TResult>> action, CancellationToken cancellationToken = default)
    {
        await BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action(cancellationToken);
            await CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await RollbackTransactionAsync(CancellationToken.None);
            throw;
        }
    }

    public void ClearTracker()
    {
        _context.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
