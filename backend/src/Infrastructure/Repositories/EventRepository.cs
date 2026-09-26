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
                .Where(e => e.Visibility == EventVisibility.Public)
                .Select(e => EventEntityToDomain.ToDomain(e))
                .ToListAsync(cancellationToken);
        return result;
    }
}