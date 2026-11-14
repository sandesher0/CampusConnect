namespace SharedKernel.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken);
    Task AcquireAdvisoryLockAsync(string scope, Guid resourceId, CancellationToken cancellationToken);
}