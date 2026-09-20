using Modules.Auth.Domain;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Modules.Auth.Ports;

public interface IAccountRepository : IBaseRepository<AccountEntity>
{
    Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Guid?> GetUserIdByAccountIdAsync(Guid accountId, CancellationToken cancellationToken);
}