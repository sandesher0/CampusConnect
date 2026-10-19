// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Communities.Facades
namespace Modules.Communities.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using Modules.Communities.Tests.TestData;
using NSubstitute;
using SharedKernel.Constants;
using Shouldly;
using Xunit;

public class SearchCommunityFacadeTests
{
    private readonly ICommunityRepository communityRepository = Substitute.For<ICommunityRepository>();
    private readonly SearchCommunityFacade sut;

    public SearchCommunityFacadeTests()
    {
        sut = new SearchCommunityFacade(communityRepository, NullLogger<SearchCommunityFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_PassesSearchKeywordAndPageRequest_ReturnsPagedResult()
    {
        var keyword = "Engineering";
        var page = new PageRequest { Page = 1, PageSize = 20 };
        var ct = new CancellationToken();
        var expectedPaged = new PagedResult<Community>(new List<Community> { CommunityTestData.CreateCommunity() }, 1, 1, 20);
        communityRepository.SearchCommunityAsync(keyword, page, ct).Returns(expectedPaged);

        var result = await sut.HandleAsync(keyword, page, ct);

        result.ShouldBe(expectedPaged);
        await communityRepository.Received(1).SearchCommunityAsync(keyword, page, ct);
    }
}
