using Microsoft.EntityFrameworkCore;
using Modules.Events.Ports;
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
}