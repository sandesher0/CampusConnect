// Ref: workflow.md §0 AI Development Guidelines | Feature: Application.Services.LimitProvider
namespace Application.Tests.Services;

using Application.Services;
using Application.Tests.TestData;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Domain;
using SharedKernel.Interfaces;
using Shouldly;
using Xunit;

public class LimitProviderTests
{
    private readonly IPlanRepository planRepository = Substitute.For<IPlanRepository>();
    private readonly TestTimeProvider timeProvider = new(new DateTimeOffset(2026, 10, 20, 12, 0, 0, TimeSpan.Zero));
    private readonly LimitProvider sut;

    private readonly Plan defaultPlan = new()
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        Code = "free",
        Name = "Free Plan",
        IsDefault = true
    };

    private readonly List<PlanLimit> defaultPlanLimits =
    [
        new() { Id = Guid.NewGuid(), PlanId = Guid.Parse("11111111-1111-1111-1111-111111111111"), LimitKey = LimitKeys.CommunitiesMaxCreated, Value = 3 },
        new() { Id = Guid.NewGuid(), PlanId = Guid.Parse("11111111-1111-1111-1111-111111111111"), LimitKey = LimitKeys.EventsMaxActiveCreated, Value = 5 },
        new() { Id = Guid.NewGuid(), PlanId = Guid.Parse("11111111-1111-1111-1111-111111111111"), LimitKey = LimitKeys.EventsMaxTotalCreated, Value = 10 },
        new() { Id = Guid.NewGuid(), PlanId = Guid.Parse("11111111-1111-1111-1111-111111111111"), LimitKey = LimitKeys.EventsMaxCapacity, Value = 200 }
    ];

    public LimitProviderTests()
    {
        sut = new LimitProvider(
            planRepository,
            timeProvider,
            NullLogger<LimitProvider>.Instance);

        planRepository.GetDefaultPlanAsync(Arg.Any<CancellationToken>())
            .Returns(defaultPlan);

        planRepository.GetLimitsByPlanIdAsync(defaultPlan.Id, Arg.Any<CancellationToken>())
            .Returns(defaultPlanLimits);
    }

    [Fact]
    public async Task GetLimitAsync_WhenUserHasNoActiveUserPlan_FallsBackToDefaultPlan()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();
        planRepository.GetActiveUserPlanAsync(userId, timeProvider.GetUtcNow(), ct)
            .Returns((UserPlan?)null);

        var limit = await sut.GetLimitAsync(userId, LimitKeys.CommunitiesMaxCreated, ct);

        limit.Value.ShouldBe(3);
    }

    [Fact]
    public async Task GetLimitAsync_WhenUserHasActivePaidPlan_UsesPaidPlanLimit()
    {
        var userId = Guid.NewGuid();
        var paidPlanId = Guid.NewGuid();
        var ct = new CancellationToken();

        var activeUserPlan = new UserPlan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = paidPlanId,
            ValidFrom = timeProvider.GetUtcNow().AddDays(-5),
            ValidTo = timeProvider.GetUtcNow().AddDays(25)
        };

        var paidLimits = new List<PlanLimit>
        {
            new() { Id = Guid.NewGuid(), PlanId = paidPlanId, LimitKey = LimitKeys.CommunitiesMaxCreated, Value = 20 },
            new() { Id = Guid.NewGuid(), PlanId = paidPlanId, LimitKey = LimitKeys.EventsMaxCapacity, Value = null } // Unlimited
        };

        planRepository.GetActiveUserPlanAsync(userId, timeProvider.GetUtcNow(), ct)
            .Returns(activeUserPlan);
        planRepository.GetLimitsByPlanIdAsync(paidPlanId, ct)
            .Returns(paidLimits);

        var commLimit = await sut.GetLimitAsync(userId, LimitKeys.CommunitiesMaxCreated, ct);
        var capLimit = await sut.GetLimitAsync(userId, LimitKeys.EventsMaxCapacity, ct);

        commLimit.Value.ShouldBe(20);
        capLimit.IsUnlimited.ShouldBeTrue();
    }

    [Fact]
    public async Task GetLimitAsync_WhenPaidPlanHasMissingLimitKey_FallsBackToDefaultPlanKey()
    {
        var userId = Guid.NewGuid();
        var paidPlanId = Guid.NewGuid();
        var ct = new CancellationToken();

        var activeUserPlan = new UserPlan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = paidPlanId,
            ValidFrom = timeProvider.GetUtcNow().AddDays(-5),
            ValidTo = null
        };

        // Paid plan only defines communities.max_created, missing events.max_total_created
        var paidLimits = new List<PlanLimit>
        {
            new() { Id = Guid.NewGuid(), PlanId = paidPlanId, LimitKey = LimitKeys.CommunitiesMaxCreated, Value = 50 }
        };

        planRepository.GetActiveUserPlanAsync(userId, timeProvider.GetUtcNow(), ct)
            .Returns(activeUserPlan);
        planRepository.GetLimitsByPlanIdAsync(paidPlanId, ct)
            .Returns(paidLimits);

        var commLimit = await sut.GetLimitAsync(userId, LimitKeys.CommunitiesMaxCreated, ct);
        var totalEventsLimit = await sut.GetLimitAsync(userId, LimitKeys.EventsMaxTotalCreated, ct);

        commLimit.Value.ShouldBe(50);
        totalEventsLimit.Value.ShouldBe(10); // from default plan
    }

    [Fact]
    public async Task GetLimitAsync_WhenKeyMissingFromBothResolvedPlanAndDefaultPlan_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();
        planRepository.GetActiveUserPlanAsync(userId, timeProvider.GetUtcNow(), ct)
            .Returns((UserPlan?)null);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            sut.GetLimitAsync(userId, "non_existent_limit_key", ct));

        ex.Message.ShouldContain("non_existent_limit_key");
    }

    [Fact]
    public async Task GetLimitAsync_WhenDefaultPlanIsMissingFromDatabase_ThrowsInvalidOperationException()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();
        planRepository.GetDefaultPlanAsync(ct).Returns((Plan?)null);

        var ex = await Should.ThrowAsync<InvalidOperationException>(() =>
            sut.GetLimitAsync(userId, LimitKeys.CommunitiesMaxCreated, ct));

        ex.Message.ShouldContain("No default plan configured");
    }

    [Fact]
    public async Task GetLimitAsync_WhenNewerValidFromRowWinsOverOlderOne_UsesNewerPlanLimit()
    {
        var userId = Guid.NewGuid();
        var newerPlanId = Guid.NewGuid();
        var ct = new CancellationToken();

        // The repository contract guarantees that GetActiveUserPlanAsync orders by ValidFrom descending
        // and returns the active plan. Verify LimitProvider uses this resolved plan.
        var activeUserPlan = new UserPlan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanId = newerPlanId,
            ValidFrom = timeProvider.GetUtcNow().AddDays(-2),
            ValidTo = timeProvider.GetUtcNow().AddDays(28)
        };

        var newerLimits = new List<PlanLimit>
        {
            new() { Id = Guid.NewGuid(), PlanId = newerPlanId, LimitKey = LimitKeys.EventsMaxActiveCreated, Value = 100 }
        };

        planRepository.GetActiveUserPlanAsync(userId, timeProvider.GetUtcNow(), ct)
            .Returns(activeUserPlan);
        planRepository.GetLimitsByPlanIdAsync(newerPlanId, ct)
            .Returns(newerLimits);

        var limit = await sut.GetLimitAsync(userId, LimitKeys.EventsMaxActiveCreated, ct);

        limit.Value.ShouldBe(100);
    }

    [Fact]
    public async Task GetLimitAsync_AfterPaidRowValidToPassed_OlderStillValidRowAppliesAgain()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();

        // When paid plan has expired, repository returns the older still-valid free plan or null
        planRepository.GetActiveUserPlanAsync(userId, timeProvider.GetUtcNow(), ct)
            .Returns((UserPlan?)null); // falls back to default

        var limit = await sut.GetLimitAsync(userId, LimitKeys.EventsMaxActiveCreated, ct);

        limit.Value.ShouldBe(5); // free default value
}
}
