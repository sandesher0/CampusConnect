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

public class GetAllPublicCommunitiesFacadeTests
{
    private readonly ICommunityRepository communityRepository = Substitute.For<ICommunityRepository>();
    private readonly GetAllPublicCommunitiesFacade sut;

    public GetAllPublicCommunitiesFacadeTests()
    {
        sut = new GetAllPublicCommunitiesFacade(communityRepository, NullLogger<GetAllPublicCommunitiesFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_PassesPageRequestAndReturnsPagedResult()
    {
        var page = new PageRequest { Page = 2, PageSize = 10 };
        var ct = new CancellationToken();
        var expectedPaged = new PagedResult<Community>(
            new List<Community> { CommunityTestData.CreateCommunity() },
            1, 2, 10);
        communityRepository.GetAllPublicCommunitiesAsync(page, ct).Returns(expectedPaged);

        var result = await sut.HandleAsync(page, ct);

        result.ShouldBe(expectedPaged);
        await communityRepository.Received(1).GetAllPublicCommunitiesAsync(page, ct);
    }
}
