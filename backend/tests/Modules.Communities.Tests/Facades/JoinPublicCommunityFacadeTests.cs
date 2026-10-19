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

public class JoinPublicCommunityFacadeTests
{
    private readonly ICommunityRepository communityRepository = Substitute.For<ICommunityRepository>();
    private readonly ICommunityMemberRepository communityMemberRepository = Substitute.For<ICommunityMemberRepository>();
    private readonly JoinPublicCommunityFacade sut;

    public JoinPublicCommunityFacadeTests()
    {
        sut = new JoinPublicCommunityFacade(
            communityRepository,
            communityMemberRepository,
            NullLogger<JoinPublicCommunityFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenCommunityNotFound_ThrowsCommunityNotFoundExceptionAndDoesNotAddMember()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        communityRepository.GetByIdAsync(communityId, ct).Returns((CommunityEntity?)null);

        var ex = await Should.ThrowAsync<CommunityNotFoundException>(() => sut.HandleAsync(communityId, userId, ct));

        ex.Message.ShouldContain(communityId.ToString());
        await communityMemberRepository.DidNotReceive().AddAsync(Arg.Any<CommunityMemberEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenCommunityExists_AddsClubMemberEntity()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var userId = CommunityTestData.DefaultUserId;
        var ct = new CancellationToken();
        communityRepository.GetByIdAsync(communityId, ct).Returns(CommunityTestData.CreateCommunityEntity(id: communityId));

        await sut.HandleAsync(communityId, userId, ct);

        await communityMemberRepository.Received(1).AddAsync(
            Arg.Is<CommunityMemberEntity>(m =>
                m.CommunityId == communityId &&
                m.UserId == userId &&
                m.MemberType == MemberType.ClubMember),
            ct);
    }
}
