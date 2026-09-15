using Modules.Auth.Domain;
using SharedKernel.Interfaces;

namespace Modules.Auth.Ports;

public interface IUserRegistrationRepository : IBaseRepository<Account>
{
}