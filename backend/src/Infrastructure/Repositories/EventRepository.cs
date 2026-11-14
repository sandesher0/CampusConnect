using Microsoft.EntityFrameworkCore;
using Modules.Events.Domain;
using Modules.Events.Ports;
using SharedKernel.Interfaces;
using SharedKernel.Constants;
using Infrastructure.Mapper.ToDomain;
using SharedKernel.Entities;

namespace Infrastructure.Repositories;

public class EventRepository : BaseRepository<AppDbContext, EventEntity>, IEventRepository
{
    public EventRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Event>> GetAllPublicEventAsync(PageRequest page, CancellationToken cancellationToken)
    {
        var pagedEntities = await dbSet.AsNoTracking()
            .Where(e => e.Visibility == EventVisibility.Public &&
                        e.DeletedAt == null &&
                        e.EventEndDate >= DateTimeOffset.UtcNow)
            .OrderBy(e => e.EventDate)
            .ToPagedAsync(page, cancellationToken);

        return pagedEntities.Map(EventEntityToDomain.ToDomain);
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

    public async Task<int> CountTotalCreatedByUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await dbSet
            .AsNoTracking()
            .Where(e => e.CreatedBy == userId)
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountActiveCreatedByUserAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        return await dbSet
            .AsNoTracking()
            .Where(e => e.CreatedBy == userId && e.DeletedAt == null && e.EventEndDate > now)
            .CountAsync(cancellationToken);
    }
    public async Task<bool> TryReserveCapacityAsync(Guid eventId, int activeReservations, CancellationToken cancellationToken)
    {
        var eventEntity = await dbSet
            .Where(e => e.Id == eventId && e.DeletedAt == null)
            .SingleOrDefaultAsync(cancellationToken);
        if (eventEntity is null)
        {
            return false;
        }

        if (activeReservations >= eventEntity.Capacity)
        {
            return false;
        }

        var affectedRows = await dbSet
            .Where(e => e.Id == eventId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    e => e.ReservedCount,
                    activeReservations + 1),
                cancellationToken);

        return affectedRows > 0;
    }
    public async Task<bool> TryFreeCapacityAsync(Guid eventId, int activeReservations, CancellationToken cancellationToken)
    {
        var eventExists = await dbSet
            .AsNoTracking()
            .AnyAsync(e => e.Id == eventId && e.DeletedAt == null, cancellationToken);
        if (!eventExists)
        {
            return false;
        }

        var affectedRows = await dbSet
            .Where(e => e.Id == eventId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    e => e.ReservedCount,
                    activeReservations),
                cancellationToken);

        return affectedRows > 0;
    }

}
