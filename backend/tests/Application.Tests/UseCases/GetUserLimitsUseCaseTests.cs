// Ref: workflow.md §0 AI Development Guidelines | Feature: Application.UseCase
namespace Application.Tests.UseCases;

using Application.Ports;
using Application.UseCase;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Ports;
using Modules.Communities.Ports;
using Modules.Events.Ports;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Domain;
using SharedKernel.Entities;
using SharedKernel.Interfaces;
using SharedKernel.Models;
using Shouldly;
using Xunit;

public class GetUserLimitsUseCaseTests
{
    private readonly IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade = Substitute.For<IGetUserIdByAccountIdFacade>();
    private readonly IPlanRepository planRepository = Substitute.For<IPlanRepository>();
    private readonly ILimitProvider limitProvider = Substitute.For<ILimitProvider>();
    private readonly ICommunityRepository communityRepository = Substitute.For<ICommunityRepository>();
    private readonly IEventRepository eventRepository = Substitute.For<IEventRepository>();
    private readonly TimeProvider timeProvider = TimeProvider.System;
    private readonly GetUserLimitsUseCase sut;

    public GetUserLimitsUseCaseTests()
    {
        sut = new GetUserLimitsUseCase(
            getUserIdByAccountIdFacade,
            planRepository,
            limitProvider,
            communityRepository,
            eventRepository,
            timeProvider,
            NullLogger<GetUserLimitsUseCase>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ReturnsLimitsWithAccurateUsageCounters()
    {
        var accountId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();

        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);
        planRepository.GetDefaultPlanAsync(ct).Returns(new Plan
        {
            Id = Guid.NewGuid(),
            Code = "free",
            Name = "Free Plan",
            IsDefault = true
        });

        var limitsDict = new Dictionary<string, Limit>
        {
            { LimitKeys.CommunitiesMaxCreated, new Limit(3) },
            { LimitKeys.EventsMaxTotalCreated, new Limit(10) },
            { LimitKeys.EventsMaxActiveCreated, new Limit(5) },
            { LimitKeys.EventsMaxCapacity, new Limit(200) }
        };

        limitProvider.GetAllLimitsAsync(userId, ct).Returns(limitsDict);
        communityRepository.CountCreatedByUserAsync(userId, ct).Returns(2);
        eventRepository.CountTotalCreatedByUserAsync(userId, ct).Returns(7);
        eventRepository.CountActiveCreatedByUserAsync(userId, Arg.Any<DateTimeOffset>(), ct).Returns(3);

        var result = await sut.HandleAsync(accountId, ct);

        result.PlanCode.ShouldBe("free");
        result.Limits[LimitKeys.CommunitiesMaxCreated].Limit.ShouldBe(3);
        result.Limits[LimitKeys.CommunitiesMaxCreated].CurrentUsage.ShouldBe(2);

        result.Limits[LimitKeys.EventsMaxTotalCreated].Limit.ShouldBe(10);
        result.Limits[LimitKeys.EventsMaxTotalCreated].CurrentUsage.ShouldBe(7);

        result.Limits[LimitKeys.EventsMaxActiveCreated].Limit.ShouldBe(5);
        result.Limits[LimitKeys.EventsMaxActiveCreated].CurrentUsage.ShouldBe(3);

        result.Limits[LimitKeys.EventsMaxCapacity].Limit.ShouldBe(200);
        result.Limits[LimitKeys.EventsMaxCapacity].CurrentUsage.ShouldBeNull();
    }
}
