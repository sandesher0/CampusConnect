using SharedKernel.Entities;
using SharedKernel.Response;

namespace Modules.Events.Mapper.ToResponse;

public static class MyEventReservationMapper
{
    public static MyEventReservationResponse ToResponse(EventReservationEntity entity)
    {
        return new MyEventReservationResponse
        {
            ReservationId  = entity.Id,
            EventId        = entity.EventId,
            ReservedAt     = entity.CreatedAt,
            Title          = entity.Event.Title,
            Description    = entity.Event.Description,
            Location       = entity.Event.Location,
            EventDate      = entity.Event.EventDate,
            EventEndDate   = entity.Event.EventEndDate,
            Visibility     = entity.Event.Visibility,
            Category       = entity.Event.Category,
            CommunityId    = entity.Event.CommunityId,
        };
    }
}
