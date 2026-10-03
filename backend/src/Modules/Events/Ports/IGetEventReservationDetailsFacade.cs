using Modules.Events.Domain;
using SharedKernel.Entities;
using SharedKernel.Response;

namespace Modules.Events.Ports;


public interface IGetEventReservationDetailsFacade
{
    Task<List<EventReservationSummaryResponse>> HandleAsync(Guid userId, Guid eventId, CancellationToken cancellationToken);
}