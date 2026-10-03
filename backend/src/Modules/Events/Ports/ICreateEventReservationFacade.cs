using Modules.Events.Domain;

namespace Modules.Events.Ports;


public interface ICreateEventReservationFacade
{
    Task HandleAsync(EventReservation domain, CancellationToken cancellationToken);
}