using Modules.Auth.Domain;

namespace Modules.Auth.Ports;

public interface IUserRegistrationFacade
{
    Task HandleAsync(Account account, CancellationToken cancellationToken);
}
