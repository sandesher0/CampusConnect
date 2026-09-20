using Modules.Events.Domain;

namespace Application.Mapper.ToModuleDomain;

public static class EventToEventsDomainMapper
{
    public static Event ToDomain(Application.Domain.Event domain , Guid communityId ,Guid createdBy )
    {
        return new Event
        {
            Title = domain.Title,
            Description = domain.Description,
            Location = domain.Location,
            EventDate = domain.EventDate,
            EventEndDate = domain.EventEndDate,
            Visibility = domain.Visibility,
            Category = domain.Category,
            CommunityId = communityId,
            CreatedBy= createdBy
        };
    }
}