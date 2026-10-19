// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Communities.Facades
namespace Modules.Communities.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Communities.Domain;
using Modules.Communities.Facades;
using Modules.Communities.Ports;
using Modules.Communities.Tests.TestData;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Entities;
using SharedKernel.Exceptions;
using Shouldly;
using Xunit;

public class EnsureCanCreateEventFacadeTests
{
    private readonly ICommunityRepository communityRepository = Substitute.For<ICommunityRepository>();
    private readonly ICommunityMemberRepository communityMemberRepository = Substitute.For<ICommunityMemberRepository>();
    private readonly EnsureCanCreateEventFacade sut;

    public EnsureCanCreateEventFacadeTests()
    {
        sut = new EnsureCanCreateEventFacade(
            communityRepository,
            communityMemberRepository,
            NullLogger<EnsureCanCreateEventFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenCommunityDoesNotExist_ThrowsCommunityNotFoundException()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        communityRepository.GetByIdAsync(communityId, ct).Returns((CommunityEntity?)null);

        var ex = await Should.ThrowAsync<CommunityNotFoundException>(() => sut.HandleAsync(communityId, userId, ct));

        ex.Message.ShouldContain(communityId.ToString());
        await communityMemberRepository.DidNotReceive()
            .GetByCommunityIdAndUserId(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsNotMemberOfCommunity_ThrowsForbiddenException()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        communityRepository.GetByIdAsync(communityId, ct).Returns(CommunityTestData.CreateCommunityEntity(id: communityId));
        communityMemberRepository.GetByCommunityIdAndUserId(communityId, userId, ct).Returns((CommunityMember?)null);

        var ex = await Should.ThrowAsync<ForbiddenException>(() => sut.HandleAsync(communityId, userId, ct));

        ex.Message.ShouldContain("must be a member");
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsClubMemberButNotOfficer_ThrowsForbiddenException()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        var regularMember = CommunityTestData.CreateCommunityMember(
            communityId: communityId,
            userId: userId,
            memberType: MemberType.ClubMember);

        communityRepository.GetByIdAsync(communityId, ct).Returns(CommunityTestData.CreateCommunityEntity(id: communityId));
        communityMemberRepository.GetByCommunityIdAndUserId(communityId, userId, ct).Returns(regularMember);

        var ex = await Should.ThrowAsync<ForbiddenException>(() => sut.HandleAsync(communityId, userId, ct));

        ex.Message.ShouldContain("Only club officers can create events");
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsClubOfficer_CompletesSuccessfully()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        var officerMember = CommunityTestData.CreateCommunityMember(
            communityId: communityId,
            userId: userId,
            memberType: MemberType.ClubOfficer);

        communityRepository.GetByIdAsync(communityId, ct).Returns(CommunityTestData.CreateCommunityEntity(id: communityId));
        communityMemberRepository.GetByCommunityIdAndUserId(communityId, userId, ct).Returns(officerMember);

        await Should.NotThrowAsync(() => sut.HandleAsync(communityId, userId, ct));

        await communityMemberRepository.Received(1).GetByCommunityIdAndUserId(communityId, userId, ct);
    }
}
