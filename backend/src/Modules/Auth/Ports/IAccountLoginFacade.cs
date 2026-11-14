using Modules.Auth.Domain;

namespace Modules.Auth.Ports;

public interface IAccountLoginFacade
{
    Task<LoginTokens> HandleAsync(Login login, CancellationToken cancellationToken);
}