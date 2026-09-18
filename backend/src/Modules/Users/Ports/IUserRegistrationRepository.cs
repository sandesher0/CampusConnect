using SharedKernel.Entities;
using SharedKernel.Interfaces;
using Modules.Users.Domain;

namespace Modules.Users.Ports;

public interface IUserRegistrationRepository : IBaseRepository<UserEntity>
{
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
}
