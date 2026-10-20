// Ref: workflow.md §0 AI Development Guidelines | Feature: Application.UseCase
namespace Application.Tests.UseCases;

using Application.Domain;
using Application.Tests.TestData;
using Application.UseCase;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Ports;
using Modules.Communities.Ports;
using Modules.Events.Policies;
using Modules.Events.Ports;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using SharedKernel.Models;
using Shouldly;
using Xunit;

public class CreateEventUseCaseTests
{
    private readonly IGetUserIdByAccountIdFacade userIdByAccountIdFacade = Substitute.For<IGetUserIdByAccountIdFacade>();
    private readonly IEnsureCanCreateEventFacade ensureCanCreateEventFacade = Substitute.For<IEnsureCanCreateEventFacade>();
    private readonly IEventCreationPolicy eventCreationPolicy = Substitute.For<IEventCreationPolicy>();
    private readonly ICreateEventFacade createEventFacade = Substitute.For<ICreateEventFacade>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateEventUseCase sut;

    public CreateEventUseCaseTests()
    {
        eventCreationPolicy.EvaluateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(PolicyResult.Success());

        sut = new CreateEventUseCase(
            userIdByAccountIdFacade,
            ensureCanCreateEventFacade,
            eventCreationPolicy,
            NullLogger<CreateEventUseCase>.Instance,
            createEventFacade,
            unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WhenAccountNotFound_ThrowsAccountNotFoundExceptionAndDoesNotCreate()
    {
        var domainEvent = ApplicationTestData.CreateEvent();
        var accountId = ApplicationTestData.DefaultAccountId;
        var ct = new CancellationToken();
        userIdByAccountIdFacade.HandleAsync(accountId, ct).Returns((Guid?)null);

        var ex = await Should.ThrowAsync<AccountNotFoundException>(() => sut.HandleAsync(domainEvent, accountId, ct));

        ex.Message.ShouldContain(accountId.ToString());
        await ensureCanCreateEventFacade.DidNotReceive()
            .HandleAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await createEventFacade.DidNotReceive()
            .HandleAsync(Arg.Any<Modules.Events.Domain.Event>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenPermissionCheckFails_ThrowsForbiddenExceptionAndDoesNotCreate()
    {
        var domainEvent = ApplicationTestData.CreateEvent();
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var ct = new CancellationToken();
        userIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);
        ensureCanCreateEventFacade.HandleAsync(domainEvent.CommunityId, userId, ct)
            .Returns(Task.FromException(new ForbiddenException("Only club officers can create events for this community.")));

        await Should.ThrowAsync<ForbiddenException>(() => sut.HandleAsync(domainEvent, accountId, ct));

        await eventCreationPolicy.DidNotReceive().EvaluateAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await createEventFacade.DidNotReceive()
            .HandleAsync(Arg.Any<Modules.Events.Domain.Event>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenPlanLimitExceeded_ThrowsPlanLimitExceededExceptionAndDoesNotCreate()
    {
        var domainEvent = ApplicationTestData.CreateEvent();
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var ct = new CancellationToken();
        userIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);
        eventCreationPolicy.EvaluateAsync(userId, ct)
            .Returns(PolicyResult.Failure("Limit reached", LimitKeys.EventsMaxActiveCreated, 5));

        var ex = await Should.ThrowAsync<PlanLimitExceededException>(() => sut.HandleAsync(domainEvent, accountId, ct));

        ex.LimitKey.ShouldBe(LimitKeys.EventsMaxActiveCreated);
        ex.Limit.ShouldBe(5);
        await createEventFacade.DidNotReceive()
            .HandleAsync(Arg.Any<Modules.Events.Domain.Event>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_ValidRequest_CreatesEventAndSaves()
    {
        var domainEvent = ApplicationTestData.CreateEvent();
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var ct = new CancellationToken();
        userIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);

        await sut.HandleAsync(domainEvent, accountId, ct);

        await ensureCanCreateEventFacade.Received(1)
            .HandleAsync(domainEvent.CommunityId, userId, ct);
        await eventCreationPolicy.Received(1)
            .EvaluateAsync(userId, ct);
        await createEventFacade.Received(1)
            .HandleAsync(Arg.Any<Modules.Events.Domain.Event>(), ct);
        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }
}
