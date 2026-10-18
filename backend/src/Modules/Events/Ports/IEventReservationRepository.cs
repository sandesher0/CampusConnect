using SharedKernel.Constants;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Modules.Events.Ports;

public interface IEventReservationRepository : IBaseRepository<EventReservationEntity>
{
    Task<EventReservationEntity?> CheckEventReservation(Guid eventId, Guid UserId, CancellationToken cancellationToken);
    Task<EventReservationEntity?> GetActiveReservationAsync(Guid eventId, Guid userId, CancellationToken cancellationToken);
    Task<PagedResult<EventReservationEntity>> GetByEventIdAsync(Guid eventId, PageRequest page, CancellationToken cancellationToken);
    Task<PagedResult<EventReservationEntity>> GetByUserIdAsync(Guid userId, PageRequest page, CancellationToken cancellationToken);
}