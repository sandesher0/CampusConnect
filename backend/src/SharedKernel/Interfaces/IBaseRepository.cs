namespace SharedKernel.Interfaces;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(T domain, CancellationToken cancellationToken);
    Task UpdateAsync(T domain, CancellationToken cancellationToken);
    Task DeleteAsync(T domain, CancellationToken cancellationToken);
}