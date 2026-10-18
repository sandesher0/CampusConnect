using Modules.Events.Domain;
using SharedKernel.Constants;
using SharedKernel.Entities;
using SharedKernel.Response;

namespace Modules.Events.Ports;

public interface IGetEventReservationDetailsFacade
{
    Task<PagedResult<EventReservationSummaryResponse>> HandleAsync(Guid userId, Guid eventId, PageRequest page, CancellationToken cancellationToken);
}