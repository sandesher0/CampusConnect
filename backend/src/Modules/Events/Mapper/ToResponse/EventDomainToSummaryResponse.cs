using Modules.Events.Domain;
using SharedKernel.Response;

namespace Modules.Events.Mapper.ToResponse;

public static class EventDomainToSummaryResponse
{
    public static EventSummaryResponse ToResponse(Event domain)
    {
        return new EventSummaryResponse
        {
            Id = domain.Id,
            Title = domain.Title,
            Location = domain.Location,
            EventDate = domain.EventDate,
            EventEndDate = domain.EventEndDate,
            Visibility = domain.Visibility,
            Category = domain.Category,
        };
    }
}
