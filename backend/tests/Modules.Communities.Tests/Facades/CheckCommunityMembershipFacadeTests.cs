// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Communities.Facades
namespace Modules.Communities.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Communities.Domain;
using Modules.Communities.Facades;
using Modules.Communities.Ports;
using Modules.Communities.Tests.TestData;
using NSubstitute;
using Shouldly;
using Xunit;

public class CheckCommunityMembershipFacadeTests
{
    private readonly ICommunityMemberRepository communityMemberRepository = Substitute.For<ICommunityMemberRepository>();
    private readonly CheckCommunityMembershipFacade sut;

    public CheckCommunityMembershipFacadeTests()
    {
        sut = new CheckCommunityMembershipFacade(communityMemberRepository, NullLogger<CheckCommunityMembershipFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsMember_ReturnsCommunityMember()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        var expectedMember = CommunityTestData.CreateCommunityMember(communityId: communityId, userId: userId);
        communityMemberRepository.GetByCommunityIdAndUserId(communityId, userId, ct).Returns(expectedMember);

        var result = await sut.HandleAsync(userId, communityId, ct);

        result.ShouldBe(expectedMember);
        await communityMemberRepository.Received(1).GetByCommunityIdAndUserId(communityId, userId, ct);
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsNotMember_ReturnsNull()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        communityMemberRepository.GetByCommunityIdAndUserId(communityId, userId, ct).Returns((CommunityMember?)null);

        var result = await sut.HandleAsync(userId, communityId, ct);

        result.ShouldBeNull();
        await communityMemberRepository.Received(1).GetByCommunityIdAndUserId(communityId, userId, ct);
    }
}
