namespace ProgressService.Domain.IRepositories
{
    public interface IUnitOfWork
    {
        Task ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default);
        Task AddSavePointAsync(string savePointName, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
