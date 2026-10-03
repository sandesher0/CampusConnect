using SharedKernel.Response;

namespace Modules.Events.Ports;

public interface IGetEventFacade
{
    Task<EventDetailResponse> HandleAsync(Guid eventId, CancellationToken cancellationToken);
}
