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

public class CommunityMembershipEligibilityFacadeTests
{
    private readonly ICommunityMemberRepository communityMemberRepository = Substitute.For<ICommunityMemberRepository>();
    private readonly CommunityMembershipEligibilityFacade sut;

    public CommunityMembershipEligibilityFacadeTests()
    {
        sut = new CommunityMembershipEligibilityFacade(
            communityMemberRepository,
            NullLogger<CommunityMembershipEligibilityFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenUserAlreadyMember_ThrowsInvalidOperationException()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        var existingMember = CommunityTestData.CreateCommunityMember(communityId: communityId, userId: userId);
        communityMemberRepository.GetByCommunityIdAndUserId(communityId, userId, ct).Returns(existingMember);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() => sut.HandleAsync(communityId, userId, ct));

        ex.Message.ShouldContain(userId.ToString());
        ex.Message.ShouldContain(communityId.ToString());
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotMember_CompletesSuccessfully()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        communityMemberRepository.GetByCommunityIdAndUserId(communityId, userId, ct).Returns((CommunityMember?)null);

        await Should.NotThrowAsync(() => sut.HandleAsync(communityId, userId, ct));

        await communityMemberRepository.Received(1).GetByCommunityIdAndUserId(communityId, userId, ct);
    }
}
