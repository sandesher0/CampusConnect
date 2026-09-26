using Modules.Events.Domain;
using SharedKernel.Response;

namespace Modules.Events.Ports;

public interface IGetAllPublicEventFacade
{
    Task<List<EventResponse>> HandleAsync(CancellationToken cancellationToken);
}