// Ref: workflow.md §0 AI Development Guidelines | Feature: TestData
namespace Application.Tests.TestData;

using Application.Domain;
using SharedKernel.Constants;

public static class ApplicationTestData
{
    public static readonly Guid DefaultAccountId = Guid.Parse("11111111-2222-3333-4444-555555555555");
    public static readonly Guid DefaultUserId = Guid.Parse("22222222-3333-4444-5555-666666666666");
    public static readonly Guid DefaultCommunityId = Guid.Parse("33333333-4444-5555-6666-777777777777");
    public static readonly Guid DefaultEventId = Guid.Parse("44444444-5555-6666-7777-888888888888");

    public static RegisterAccount CreateRegisterAccount(
        string email = "newuser@campus.edu",
        string username = "newuser",
        string password = "Password123!",
        string firstName = "Alex",
        string lastName = "Rivera") =>
        new()
        {
            Email = email,
            Username = username,
            Password = password,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = "555-9876"
        };

    public static Event CreateEvent(
        Guid? id = null,
        Guid? communityId = null,
        string title = "Tech Symposium") =>
        new()
        {
            Id = id ?? DefaultEventId,
            CommunityId = communityId ?? DefaultCommunityId,
            Title = title,
            Description = "Campus Tech Symposium",
            Location = "Hall B",
            EventDate = DateTimeOffset.Parse("2026-12-01T10:00:00Z"),
            EventEndDate = DateTimeOffset.Parse("2026-12-01T16:00:00Z"),
            Visibility = EventVisibility.Public,
            Category = EventCategory.Academic
        };
}
