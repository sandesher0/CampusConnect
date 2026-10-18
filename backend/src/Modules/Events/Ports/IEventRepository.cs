

using SharedKernel.Interfaces;
using SharedKernel.Entities;
using Modules.Events.Domain;
using SharedKernel.Constants;

namespace Modules.Events.Ports;

public interface IEventRepository : IBaseRepository<EventEntity>
{
    Task<PagedResult<Event>> GetAllPublicEventAsync(PageRequest page, CancellationToken cancellationToken);
    Task<EventEntity?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken);
}