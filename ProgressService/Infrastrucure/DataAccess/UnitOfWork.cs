using Microsoft.EntityFrameworkCore.Storage;
using ProgressService.Domain.IRepositories;
using ProgressService.Infrastrucure.Persistence.Configurations;

namespace ProgressService.Infrastrucure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ProgressDbContext _context;
        private IDbContextTransaction? _transaction;
        private int _depth = 0;
        private string? _currentSavePoint;

        public UnitOfWork(ProgressDbContext context) => _context = context;

        public async Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            var isOutermost = _depth == 0;

            if (isOutermost)
            {
                _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                _currentSavePoint = null;
            }

            _depth++;

            try
            {
                await action();

                if (isOutermost)
                {
                    await _context.SaveChangesAsync(cancellationToken);
                    await _transaction!.CommitAsync(cancellationToken);
                }
            }
            catch
            {
                if (isOutermost)
                {
                    await _transaction!.RollbackAsync(CancellationToken.None);
                    _context.ChangeTracker.Clear();
                }
                throw;
            }
            finally
            {
                _depth--;

                if (isOutermost)
                {
                    if (_transaction != null)
                    {
                        await _transaction.DisposeAsync();
                        _transaction = null;
                    }
                    _currentSavePoint = null;
                }
            }
        }

        public async Task AddSavePointAsync(string savePointName, CancellationToken cancellationToken = default)
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to create a savepoint.");

            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CreateSavepointAsync(savePointName, cancellationToken);
            _currentSavePoint = savePointName;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => _context.SaveChangesAsync(cancellationToken);
    }
}
