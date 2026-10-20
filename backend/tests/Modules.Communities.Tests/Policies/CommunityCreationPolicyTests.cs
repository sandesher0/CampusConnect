// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Communities.Policies
namespace Modules.Communities.Tests.Policies;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Communities.Policies;
using Modules.Communities.Ports;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Interfaces;
using SharedKernel.Models;
using Shouldly;
using Xunit;

public class CommunityCreationPolicyTests
{
    private readonly ICommunityRepository communityRepository = Substitute.For<ICommunityRepository>();
    private readonly ILimitProvider limitProvider = Substitute.For<ILimitProvider>();
    private readonly CommunityCreationPolicy sut;

    public CommunityCreationPolicyTests()
    {
        sut = new CommunityCreationPolicy(
            communityRepository,
            limitProvider,
            NullLogger<CommunityCreationPolicy>.Instance);
    }

    [Fact]
    public async Task EvaluateAsync_WhenLimitIsUnlimited_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();
        limitProvider.GetLimitAsync(userId, LimitKeys.CommunitiesMaxCreated, ct)
            .Returns(new Limit(null));

        var result = await sut.EvaluateAsync(userId, ct);

        result.Succeeded.ShouldBeTrue();
        await communityRepository.DidNotReceive().CountCreatedByUserAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EvaluateAsync_WhenCountIsBelowLimit_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();
        limitProvider.GetLimitAsync(userId, LimitKeys.CommunitiesMaxCreated, ct)
            .Returns(new Limit(3));
        communityRepository.CountCreatedByUserAsync(userId, ct)
            .Returns(2);

        var result = await sut.EvaluateAsync(userId, ct);

        result.Succeeded.ShouldBeTrue();
    }

    [Fact]
    public async Task EvaluateAsync_WhenCountEqualsOrExceedsLimit_ReturnsFailure()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();
        limitProvider.GetLimitAsync(userId, LimitKeys.CommunitiesMaxCreated, ct)
            .Returns(new Limit(3));
        communityRepository.CountCreatedByUserAsync(userId, ct)
            .Returns(3);

        var result = await sut.EvaluateAsync(userId, ct);

        result.Succeeded.ShouldBeFalse();
        result.LimitKey.ShouldBe(LimitKeys.CommunitiesMaxCreated);
        result.Limit.ShouldBe(3);
    }
}
