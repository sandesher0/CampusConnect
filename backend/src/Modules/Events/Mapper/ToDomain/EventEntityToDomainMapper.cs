using Modules.Events.Domain;
using SharedKernel.Entities;

namespace Modules.Events.Mapper.ToDomain;

internal static class EventEntityToDomainMapper
{
    public static Event ToDomain(this EventEntity entity)
    {
        return Event.Hydrate(
            entity.Id,
            entity.Title,
            entity.Description,
            entity.Location,
            entity.CommunityId,
            entity.CreatedBy,
            entity.EventDate,
            entity.EventEndDate,
            entity.Visibility,
            entity.Category);
    }
}