using Modules.Events.Domain;

namespace Modules.Mapper.ToEntity;

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
            EventEndDate = domain.EventDate,
            Visibility = domain.Visibility,
            Category = domain.Category,
            CreateAt = DateTimeOffset.UtcNow
        };
    }
}