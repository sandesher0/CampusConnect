// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Events.Policies
namespace Modules.Events.Tests.Policies;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Events.Policies;
using Modules.Events.Ports;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Interfaces;
using SharedKernel.Models;
using Shouldly;
using Xunit;

public class EventCreationPolicyTests
{
    private readonly IEventRepository eventRepository = Substitute.For<IEventRepository>();
    private readonly ILimitProvider limitProvider = Substitute.For<ILimitProvider>();
    private readonly TimeProvider timeProvider = TimeProvider.System;
    private readonly EventCreationPolicy sut;

    public EventCreationPolicyTests()
    {
        sut = new EventCreationPolicy(
            eventRepository,
            limitProvider,
            timeProvider,
            NullLogger<EventCreationPolicy>.Instance);
    }

    [Fact]
    public async Task EvaluateAsync_WhenTotalLimitExceeded_FailsFastOnTotalLimit()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();

        limitProvider.GetLimitAsync(userId, LimitKeys.EventsMaxTotalCreated, ct)
            .Returns(new Limit(10));
        eventRepository.CountTotalCreatedByUserAsync(userId, ct)
            .Returns(10); // reached total limit

        var result = await sut.EvaluateAsync(userId, ct);

        result.Succeeded.ShouldBeFalse();
        result.LimitKey.ShouldBe(LimitKeys.EventsMaxTotalCreated);
        result.Limit.ShouldBe(10);

        // Active limit check should not even be evaluated if total fails
        await limitProvider.DidNotReceive().GetLimitAsync(userId, LimitKeys.EventsMaxActiveCreated, ct);
    }

    [Fact]
    public async Task EvaluateAsync_WhenTotalAllowed_ButActiveLimitExceeded_FailsOnActiveLimit()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();

        limitProvider.GetLimitAsync(userId, LimitKeys.EventsMaxTotalCreated, ct)
            .Returns(new Limit(10));
        eventRepository.CountTotalCreatedByUserAsync(userId, ct)
            .Returns(5); // total count ok

        limitProvider.GetLimitAsync(userId, LimitKeys.EventsMaxActiveCreated, ct)
            .Returns(new Limit(5));
        eventRepository.CountActiveCreatedByUserAsync(userId, Arg.Any<DateTimeOffset>(), ct)
            .Returns(5); // active count reached

        var result = await sut.EvaluateAsync(userId, ct);

        result.Succeeded.ShouldBeFalse();
        result.LimitKey.ShouldBe(LimitKeys.EventsMaxActiveCreated);
        result.Limit.ShouldBe(5);
    }

    [Fact]
    public async Task EvaluateAsync_WhenSoftDeletedEventsExist_CountOnlyTowardTotalNotActive()
    {
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();

        // 8 total created (e.g. 5 active + 3 soft-deleted)
        limitProvider.GetLimitAsync(userId, LimitKeys.EventsMaxTotalCreated, ct)
            .Returns(new Limit(10));
        eventRepository.CountTotalCreatedByUserAsync(userId, ct)
            .Returns(8);

        // 3 active events currently
        limitProvider.GetLimitAsync(userId, LimitKeys.EventsMaxActiveCreated, ct)
            .Returns(new Limit(5));
        eventRepository.CountActiveCreatedByUserAsync(userId, Arg.Any<DateTimeOffset>(), ct)
            .Returns(3);

        var result = await sut.EvaluateAsync(userId, ct);

        result.Succeeded.ShouldBeTrue();
    }
}
