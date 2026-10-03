using Modules.Events.Domain;
using SharedKernel.Entities;
using SharedKernel.Response;

namespace Modules.Events.Mapper.ToResponse;

public static class EventEntityToDetailResponse
{
    public static EventDetailResponse ToResponse(EventEntity eventEntity)
    {
        return new EventDetailResponse
        {
            Id = eventEntity.Id,
            Title = eventEntity.Title,
            Description = eventEntity.Description,
            Location = eventEntity.Location,
            CommunityId = eventEntity.CommunityId,
            CreatedBy = eventEntity.CreatedBy,
            EventDate = eventEntity.EventDate,
            EventEndDate = eventEntity.EventEndDate,
            Visibility = eventEntity.Visibility,
            Category = eventEntity.Category
        };
    }
}
