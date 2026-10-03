using SharedKernel.Entities;
using SharedKernel.Response;

namespace Modules.Events.Mapper.ToResponse;

public static class EventReservationSummaryMapper
{
    public static EventReservationSummaryResponse ToSummaryResponse(EventReservationEntity entity)
    {
        return new EventReservationSummaryResponse
        {
            Id = entity.Id,
            UserId = entity.UserId,
            ReservedAt = entity.CreatedAt,
            FirstName = entity.User.FirstName,
            LastName = entity.User.LastName,
            Email = entity.User.Email,
            ProfileImageUrl = entity.User.ProfileImageUrl
        };
    }
}