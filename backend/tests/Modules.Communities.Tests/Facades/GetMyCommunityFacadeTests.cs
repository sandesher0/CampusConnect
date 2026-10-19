// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Communities.Facades
namespace Modules.Communities.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Communities.Domain;
using Modules.Communities.Facades;
using Modules.Communities.Ports;
using Modules.Communities.Tests.TestData;
using NSubstitute;
using SharedKernel.Constants;
using Shouldly;
using Xunit;

public class GetMyCommunityFacadeTests
{
    private readonly ICommunityRepository repository = Substitute.For<ICommunityRepository>();
    private readonly GetMyCommunityFacade sut;

    public GetMyCommunityFacadeTests()
    {
        sut = new GetMyCommunityFacade(repository, NullLogger<GetMyCommunityFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_PassesUserIdAndPageRequest_ReturnsPagedCommunities()
    {
        var userId = CommunityTestData.DefaultUserId;
        var page = new PageRequest { Page = 1, PageSize = 15 };
        var ct = new CancellationToken();
        var expectedPaged = new PagedResult<Community>(new List<Community> { CommunityTestData.CreateCommunity() }, 1, 1, 15);
        repository.GetMyCommunitiesAsync(userId, page, ct).Returns(expectedPaged);

        var result = await sut.HandleAsync(userId, page, ct);

        result.ShouldBe(expectedPaged);
        await repository.Received(1).GetMyCommunitiesAsync(userId, page, ct);
    }
}
