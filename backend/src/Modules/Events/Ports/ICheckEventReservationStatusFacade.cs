namespace Modules.Events.Ports;

public interface ICheckEventReservationStatusFacade
{
    Task<bool> HandleAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);
}