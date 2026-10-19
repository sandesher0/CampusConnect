// Ref: workflow.md §0 AI Development Guidelines | Feature: TestData
namespace Modules.Events.Tests.TestData;

using Modules.Events.Domain;
using SharedKernel.Constants;
using SharedKernel.Entities;

public static class EventTestData
{
    public static readonly Guid DefaultEventId = Guid.Parse("66666666-6666-6666-6666-666666666666");
    public static readonly Guid DefaultCommunityId = Guid.Parse("77777777-7777-7777-7777-777777777777");
    public static readonly Guid DefaultUserId = Guid.Parse("88888888-8888-8888-8888-888888888888");
    public static readonly Guid DefaultReservationId = Guid.Parse("99999999-9999-9999-9999-999999999999");

    public static Event CreateEvent(
        Guid? id = null,
        string title = "Campus Hackathon",
        Guid? communityId = null,
        Guid? createdBy = null,
        EventVisibility visibility = EventVisibility.Public,
        EventCategory category = EventCategory.Academic) =>
        new()
        {
            Id = id ?? DefaultEventId,
            Title = title,
            Description = "Annual campus hackathon",
            Location = "Main Auditorium",
            EventDate = DateTimeOffset.Parse("2026-11-01T09:00:00Z"),
            EventEndDate = DateTimeOffset.Parse("2026-11-01T18:00:00Z"),
            Visibility = visibility,
            Category = category,
            CommunityId = communityId ?? DefaultCommunityId,
            CreatedBy = createdBy ?? DefaultUserId
        };

    public static EventEntity CreateEventEntity(
        Guid? id = null,
        string title = "Campus Hackathon",
        Guid? communityId = null,
        Guid? createdBy = null,
        EventVisibility visibility = EventVisibility.Public,
        EventCategory category = EventCategory.Academic) =>
        new()
        {
            Id = id ?? DefaultEventId,
            Title = title,
            Description = "Annual campus hackathon",
            Location = "Main Auditorium",
            EventDate = DateTimeOffset.Parse("2026-11-01T09:00:00Z"),
            EventEndDate = DateTimeOffset.Parse("2026-11-01T18:00:00Z"),
            Visibility = visibility,
            Category = category,
            CommunityId = communityId ?? DefaultCommunityId,
            CreatedBy = createdBy ?? DefaultUserId,
            CreateAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            User = new UserEntity
            {
                Id = createdBy ?? DefaultUserId,
                Email = "organizer@campus.edu",
                FirstName = "Bob",
                LastName = "Jones",
                PhoneNumber = "555-0123",
                ProfileImageUrl = "https://campus.edu/bob.png",
                CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
            },
            Community = new CommunityEntity
            {
                Id = communityId ?? DefaultCommunityId,
                CommunityName = "Engineering Society",
                CommunityType = CommunityType.Public,
                CreatedBy = createdBy ?? DefaultUserId,
                CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
                Status = CommunityStatusType.Active
            }
        };

    public static EventReservation CreateEventReservation(
        Guid? eventId = null,
        Guid? communityId = null,
        Guid? userId = null) =>
        new()
        {
            EventId = eventId ?? DefaultEventId,
            CommunityId = communityId ?? DefaultCommunityId,
            UserId = userId ?? DefaultUserId
        };

    public static EventReservationEntity CreateEventReservationEntity(
        Guid? id = null,
        Guid? eventId = null,
        Guid? communityId = null,
        Guid? userId = null,
        DateTimeOffset? deletedAt = null) =>
        new()
        {
            Id = id ?? DefaultReservationId,
            EventId = eventId ?? DefaultEventId,
            CommunityId = communityId ?? DefaultCommunityId,
            UserId = userId ?? DefaultUserId,
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            DeletedAt = deletedAt,
            User = new UserEntity
            {
                Id = userId ?? DefaultUserId,
                Email = "attendee@campus.edu",
                FirstName = "Carol",
                LastName = "White",
                CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
            },
            Event = CreateEventEntity(id: eventId ?? DefaultEventId, communityId: communityId ?? DefaultCommunityId)
        };
}
