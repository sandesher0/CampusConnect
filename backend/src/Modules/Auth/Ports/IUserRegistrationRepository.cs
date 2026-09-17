using Modules.Auth.Domain;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Modules.Auth.Ports;

public interface IUserRegistrationRepository : IBaseRepository<AccountEntity>
{
    Task <Account?> GetByEmailAsync(string email, CancellationToken cancellationToken);
}