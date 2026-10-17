using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Modules.Events.Ports;

public interface IEventReservationRepository : IBaseRepository<EventReservationEntity>
{
    Task<EventReservationEntity?> CheckEventReservation(Guid eventId, Guid UserId, CancellationToken cancellationToken);
    Task<EventReservationEntity?> GetActiveReservationAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);
    Task<List<EventReservationEntity>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
    Task<List<EventReservationEntity>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}