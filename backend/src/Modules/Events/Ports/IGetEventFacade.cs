using SharedKernel.Response;

namespace Modules.Events.Ports;

public interface IGetEventFacade
{
    Task<EventResponse> HandleAsync(Guid eventId, CancellationToken cancellationToken);
}