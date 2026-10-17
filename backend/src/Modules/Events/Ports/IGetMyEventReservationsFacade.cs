using SharedKernel.Response;

namespace Modules.Events.Ports;

public interface IGetMyEventReservationsFacade
{
    Task<List<MyEventReservationResponse>> HandleAsync(Guid userId, CancellationToken cancellationToken);
}
