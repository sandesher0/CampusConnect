using Modules.Auth.Domain;

namespace Modules.Auth.Ports;

public interface IChangePasswordFacade
{
    Task HandleAsync(Guid accountId, ChangePassword resetPassword, CancellationToken cancellationToken);
}