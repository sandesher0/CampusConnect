using SharedKernel.Interfaces;

namespace Modules.Events.Ports;

public interface ICancelEventReservationFacade
{
    Task HandleAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);
}
