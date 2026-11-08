

using SharedKernel.Interfaces;
using SharedKernel.Entities;
using Modules.Events.Domain;
using SharedKernel.Constants;

namespace Modules.Events.Ports;

public interface IEventRepository : IBaseRepository<EventEntity>
{
    Task<PagedResult<Event>> GetAllPublicEventAsync(PageRequest page, CancellationToken cancellationToken);
    Task<EventEntity?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
    Task<int> CountTotalCreatedByUserAsync(Guid userId, CancellationToken cancellationToken);
    Task<int> CountActiveCreatedByUserAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken);
    Task<bool> TryReserveCapacityAsync(Guid eventId,CancellationToken cancellationToken);
    Task<bool> TryFreeCapacityAsync(Guid eventId,CancellationToken cancellationToken);
}