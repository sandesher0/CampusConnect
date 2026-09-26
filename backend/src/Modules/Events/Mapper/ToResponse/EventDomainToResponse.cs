using Modules.Events.Domain;
using SharedKernel.Response;

namespace Modules.Events.Mapper.ToResponse;

public static class EventDomainToResponse
{
    public static EventResponse ToResponse(Event domain)
    {
        return new EventResponse
        {
            Id = domain.Id,
            Title = domain.Title,
            Description = domain.Description,
            Location = domain.Location,
            CommunityId = domain.CommunityId,
            CreatedBy = domain.CreatedBy,
            EventDate = domain.EventDate,
            EventEndDate = domain.EventDate,
            Visibility = domain.Visibility,
            Category = domain.Category,
        };
    }
}