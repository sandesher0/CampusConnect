using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Modules.Events.Ports;

public interface IEventReservationRepository : IBaseRepository<EventReservationEntity>
{
    Task<EventReservationEntity?> CheckEventReservation(Guid eventId, Guid UserId, CancellationToken cancellationToken);
}