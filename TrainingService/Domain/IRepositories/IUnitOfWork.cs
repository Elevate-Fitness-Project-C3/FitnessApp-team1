
namespace TrainingService.Domain.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task ExecuteAsync(Func<Task> action);
        Task AddSavePointAsync(string name);
        Task RollbackToSavePointAsync(string name);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}


