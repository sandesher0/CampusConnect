using Modules.Events.Domain;
using SharedKernel.Constants;
using SharedKernel.Response;

namespace Modules.Events.Ports;

public interface IGetAllPublicEventFacade
{
    Task<PagedResult<EventSummaryResponse>> HandleAsync(PageRequest page, CancellationToken cancellationToken);
}
