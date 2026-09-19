using Modules.Auth.Domain;

namespace Modules.Auth.Ports;

public interface IAccountLoginFacade
{
    Task<string> HandleAsync(Login login, CancellationToken cancellationToken);
}