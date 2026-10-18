using SharedKernel.Constants;
using SharedKernel.Response;

namespace Modules.Events.Ports;

public interface IGetMyEventReservationsFacade
{
    Task<PagedResult<MyEventReservationResponse>> HandleAsync(Guid userId, PageRequest page, CancellationToken cancellationToken);
}
