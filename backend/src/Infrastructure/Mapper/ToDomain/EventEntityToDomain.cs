using Modules.Events.Domain;

namespace Infrastructure.Mapper.ToDomain;

public static class EventEntityToDomain
{
    public static Event ToDomain(EventEntity entity)
    {
        return new Event
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            Location = entity.Location,
            EventDate = entity.EventDate,
            EventEndDate = entity.EventEndDate,
            CreatedBy = entity.CreatedBy,
            CommunityId= entity.CommunityId,
            Visibility = entity.Visibility,
            Category = entity.Category
        };
    }
}