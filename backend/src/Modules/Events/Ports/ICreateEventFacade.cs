

using Modules.Events.Domain;

namespace Modules.Events.Ports;

public interface ICreateEventFacade
{
    Task HandleAsync(Event domain, CancellationToken cancellationToken);
}