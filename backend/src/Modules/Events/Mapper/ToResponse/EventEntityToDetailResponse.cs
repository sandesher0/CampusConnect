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
            Category = eventEntity.Category,
            Organizer = new EventOrganizerUserResponse
            {
                Id = eventEntity.User.Id,
                Email = eventEntity.User.Email,
                FirstName = eventEntity.User.FirstName,
                LastName = eventEntity.User.LastName,
                PhoneNumber = eventEntity.User.PhoneNumber,
                ProfileImageUrl = eventEntity.User.ProfileImageUrl
            },
            OrganizerCommunity = new EventOrganizerCommunityResponse
            {
                CommunityId = eventEntity.Community.Id,
                CommunityName = eventEntity.Community.CommunityName,
                CommunityType = eventEntity.Community.CommunityType
            }
        };
    }
}
