using Application.Domain;
using Api.Requests;

namespace Api.Mapper.ToDomain;

public static class EventRequestToDomain
{
    public static Event ToDomain(EventRequest request)
    {
        return new Event
        {
            Title = request.Title,
            Description = request.Description,
            Location = request.Location,
            EventDate = request.EventDate,
            EventEndDate = request.EventEndDate,
            Visibility = request.Visibility,
            Category = request.Category,
            CommunityId = request.CommunityId
        };
    }
}