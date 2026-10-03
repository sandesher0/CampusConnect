using Microsoft.EntityFrameworkCore;
using Modules.Events.Domain;
using Modules.Events.Ports;
using SharedKernel.Interfaces;
using SharedKernel.Constants;
using Infrastructure.Mapper.ToDomain;

namespace Infrastructure.Repositories;

public class EventRepository : BaseRepository<AppDbContext, EventEntity>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Event>> GetAllPublicEventAsync(CancellationToken cancellationToken)
    {

        var result = await dbSet.AsNoTracking()
                .Where(e => e.Visibility == EventVisibility.Public &&
                       e.EventEndDate >= DateTimeOffset.UtcNow)
                .Select(e => EventEntityToDomain.ToDomain(e))
                .ToListAsync(cancellationToken);
        return result;
    }

    public async Task<EventEntity?> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var eventDetail = await dbSet.AsNoTracking()
                    .Where(e => e.Id == eventId &&
                    e.DeletedAt == null &&
                    e.Visibility == EventVisibility.Public)
                    .Include(e => e.User)
                    .Include(e => e.Community)
                    .SingleOrDefaultAsync(cancellationToken);
        return eventDetail;
    }
}