using Modules.Events.Domain;
using SharedKernel.Entities;

namespace Modules.Events.Mapper.ToEntity;

public static class EventReservationDomainToEntity
{
    public static EventReservationEntity ToEntity(EventReservation domain)
    {
        return new EventReservationEntity
        {
            Id = Guid.CreateVersion7(),
            EventId = domain.EventId,
            CommunityId = domain.CommunityId,
            UserId = domain.UserId,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}