using Microsoft.EntityFrameworkCore;
using Modules.Events.Ports;
using SharedKernel.Constants;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Infrastructure.Repositories;


public class EventReservationRepository : BaseRepository<AppDbContext, EventReservationEntity>, IEventReservationRepository
{
    public EventReservationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<EventReservationEntity?> CheckEventReservation(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        var eventMembership = await dbSet.AsNoTracking()
                        .Where(em => em.EventId == eventId &&
                               em.UserId == userId &&
                               em.DeletedAt == null).SingleOrDefaultAsync(cancellationToken);
        return eventMembership;
    }

    public async Task<EventReservationEntity?> GetActiveReservationAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        return await dbSet
            .Where(em => em.EventId == eventId &&
                         em.UserId == userId &&
                         em.DeletedAt == null)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedResult<EventReservationEntity>> GetByEventIdAsync(Guid eventId, PageRequest page, CancellationToken cancellationToken)
    {
        return await dbSet
            .AsNoTracking()
            .Where(r => r.EventId == eventId && r.DeletedAt == null)
            .Include(r => r.User)
            .OrderBy(r => r.CreatedAt)
            .ToPagedAsync(page, cancellationToken);
    }

    public async Task<PagedResult<EventReservationEntity>> GetByUserIdAsync(Guid userId, PageRequest page, CancellationToken cancellationToken)
    {
        return await dbSet
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.DeletedAt == null)
            .Include(r => r.Event)
            .OrderByDescending(r => r.CreatedAt)
            .ToPagedAsync(page, cancellationToken);
    }
}