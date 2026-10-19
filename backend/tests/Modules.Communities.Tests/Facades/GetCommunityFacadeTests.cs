// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Communities.Facades
namespace Modules.Communities.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Communities.Facades;
using Modules.Communities.Ports;
using Modules.Communities.Tests.TestData;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Entities;
using SharedKernel.Exceptions;
using Shouldly;
using Xunit;

public class GetCommunityFacadeTests
{
    private readonly ICommunityRepository communityRepository = Substitute.For<ICommunityRepository>();
    private readonly ICommunityMemberRepository communityMemberRepository = Substitute.For<ICommunityMemberRepository>();
    private readonly GetCommunityFacade sut;

    public GetCommunityFacadeTests()
    {
        sut = new GetCommunityFacade(
            communityRepository,
            communityMemberRepository,
            NullLogger<GetCommunityFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenCommunityNotFound_ThrowsCommunityNotFoundException()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var page = new PageRequest();
        var ct = new CancellationToken();
        communityRepository.GetByIdAsync(communityId, ct).Returns((CommunityEntity?)null);

        var ex = await Should.ThrowAsync<CommunityNotFoundException>(() => sut.HandleAsync(communityId, page, ct));

        ex.Message.ShouldContain(communityId.ToString());
        await communityMemberRepository.DidNotReceive()
            .GetByCommunityIdAsync(Arg.Any<Guid>(), Arg.Any<PageRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenCommunityExists_ReturnsCommunityDetailResponseWithPagedMembers()
    {
        var communityId = CommunityTestData.DefaultCommunityId;
        var community = CommunityTestData.CreateCommunityEntity(id: communityId);
        var page = new PageRequest { Page = 1, PageSize = 10 };
        var ct = new CancellationToken();
        var member = CommunityTestData.CreateCommunityMemberEntity(communityId: communityId);
        var pagedMembers = new PagedResult<CommunityMemberEntity>(new List<CommunityMemberEntity> { member }, 1, 1, 10);

        communityRepository.GetByIdAsync(communityId, ct).Returns(community);
        communityMemberRepository.GetByCommunityIdAsync(communityId, page, ct).Returns(pagedMembers);

        var response = await sut.HandleAsync(communityId, page, ct);

        response.ShouldNotBeNull();
        response.CommunityId.ShouldBe(community.Id);
        response.CommunityName.ShouldBe(community.CommunityName);
        response.CommunityType.ShouldBe(community.CommunityType);
        response.CreatedBy.ShouldBe(community.CreatedBy);
        response.CommunityMembers.totalCount.ShouldBe(1);
        response.CommunityMembers.Items.Count.ShouldBe(1);
        response.CommunityMembers.Items[0].UserId.ShouldBe(member.UserId);
        response.CommunityMembers.Items[0].User.Email.ShouldBe(member.User.Email);
    }
}
