using Modules.Users.Domain;

namespace Modules.Users.Ports;

public interface IUserRegistrationFacade
{
    Task HandleAsync(User user, CancellationToken cancellationToken = default);
}
