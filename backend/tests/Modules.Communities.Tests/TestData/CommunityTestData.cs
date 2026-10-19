// Ref: workflow.md §0 AI Development Guidelines | Feature: TestData
namespace Modules.Communities.Tests.TestData;

using Modules.Communities.Domain;
using SharedKernel.Constants;
using SharedKernel.Entities;

public static class CommunityTestData
{
    public static readonly Guid DefaultCommunityId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid DefaultUserId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    public static readonly Guid DefaultMemberId = Guid.Parse("55555555-5555-5555-5555-555555555555");

    public static Community CreateCommunity(
        Guid? id = null,
        string name = "Computer Science Club",
        CommunityType type = CommunityType.Public,
        Guid? createdBy = null,
        CommunityStatusType status = CommunityStatusType.Active) =>
        new()
        {
            Id = id ?? DefaultCommunityId,
            CommunityName = name,
            CommunityType = type,
            CreatedBy = createdBy ?? DefaultUserId,
            Status = status
        };

    public static CommunityEntity CreateCommunityEntity(
        Guid? id = null,
        string name = "Computer Science Club",
        CommunityType type = CommunityType.Public,
        Guid? createdBy = null,
        CommunityStatusType status = CommunityStatusType.Active) =>
        new()
        {
            Id = id ?? DefaultCommunityId,
            CommunityName = name,
            CommunityType = type,
            CreatedBy = createdBy ?? DefaultUserId,
            Status = status,
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
        };

    public static CommunityMember CreateCommunityMember(
        Guid? id = null,
        Guid? communityId = null,
        Guid? userId = null,
        MemberType memberType = MemberType.ClubMember) =>
        new()
        {
            Id = id ?? DefaultMemberId,
            CommunityId = communityId ?? DefaultCommunityId,
            UserId = userId ?? DefaultUserId,
            MemberType = memberType
        };

    public static CommunityMemberEntity CreateCommunityMemberEntity(
        Guid? id = null,
        Guid? communityId = null,
        Guid? userId = null,
        MemberType memberType = MemberType.ClubMember) =>
        new()
        {
            Id = id ?? DefaultMemberId,
            CommunityId = communityId ?? DefaultCommunityId,
            UserId = userId ?? DefaultUserId,
            MemberType = memberType,
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            User = new UserEntity
            {
                Id = userId ?? DefaultUserId,
                Email = "member@campus.edu",
                FirstName = "Alice",
                LastName = "Smith",
                PhoneNumber = "1234567890",
                ProfileImageUrl = "https://campus.edu/alice.png",
                CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
            }
        };
}
