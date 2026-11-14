using Modules.Events.Domain;

namespace Modules.Events.Mapper.ToEntity;

public static class EventDomainToEntityMapper
{
    public static EventEntity ToEntity(Event domain)
    {
        return new EventEntity
        {
            Id = Guid.CreateVersion7(),
            Title = domain.Title,
            Description = domain.Description,
            Location = domain.Location,
            CommunityId = domain.CommunityId,
            CreatedBy = domain.CreatedBy,
            EventDate = domain.EventDate,
            EventEndDate = domain.EventEndDate,
            Visibility = domain.Visibility,
            Capacity = domain.Capacity,
            ReservedCount = domain.ReservedCount,
            Category = domain.Category,
            CreateAt = DateTimeOffset.UtcNow
        };
    }
}