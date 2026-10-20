using SharedKernel.Models;

namespace SharedKernel.Interfaces;

public interface ILimitProvider
{
    Task<Limit> GetLimitAsync(Guid userId, string limitKey, CancellationToken cancellationToken = default);
    Task<IReadOnlyDictionary<string, Limit>> GetAllLimitsAsync(Guid userId, CancellationToken cancellationToken = default);
}
