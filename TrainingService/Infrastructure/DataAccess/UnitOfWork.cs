using Microsoft.EntityFrameworkCore.Storage;
using TrainingService.Domain.IRepositories;
using TrainingService.Infrastructure.Persistence;

namespace TrainingService.Infrastructure.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {

        private readonly TrainingDbContext _context;    
        private IDbContextTransaction? _transaction;
        private int _depth = 0;

        public UnitOfWork(TrainingDbContext  context) => _context = context;

        public async Task ExecuteAsync(Func<Task> action)
        {
            var isOuterTransaction = _depth == 0;

            if (isOuterTransaction)
                _transaction = await _context.Database.BeginTransactionAsync();

            _depth++;

            try
            {
                await action();

                if (isOuterTransaction)
                {
                    await _context.SaveChangesAsync();
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                if (isOuterTransaction)
                {
                    await _transaction.RollbackAsync();
                }

                throw;
            }

            finally
            {
                _depth--;
                if (isOuterTransaction)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }

        }


        public async Task AddSavePointAsync(string name)
        {
            await _context.SaveChangesAsync();
            await _transaction.CreateSavepointAsync(name);
        }

        public async Task RollbackToSavePointAsync(string name)
        {
            await _transaction.RollbackToSavepointAsync(name);

        }

        public void Dispose()
        {
            _transaction?.Dispose();
        }

    }
}