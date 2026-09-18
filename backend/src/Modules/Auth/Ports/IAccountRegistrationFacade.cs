using Modules.Auth.Domain;

namespace Modules.Auth.Ports;

public interface IAccountRegistrationFacade
{
    Task HandleAsync(Account account, CancellationToken cancellationToken);
}
