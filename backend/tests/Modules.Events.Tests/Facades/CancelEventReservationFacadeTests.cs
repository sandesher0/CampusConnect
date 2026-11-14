// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Events.Facades
namespace Modules.Events.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Events.Facades;
using Modules.Events.Ports;
using NSubstitute;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using Shouldly;
using Xunit;

public class CancelEventReservationFacadeTests
{
    private const string ReservationLockScope = "Events.Reservations";

    private readonly IEventReservationRepository eventReservationRepository = Substitute.For<IEventReservationRepository>();
    private readonly IEventRepository eventRepository = Substitute.For<IEventRepository>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CancelEventReservationFacade sut;

    public CancelEventReservationFacadeTests()
    {
        unitOfWork
            .ExecuteInTransactionAsync(Arg.Any<Func<CancellationToken, Task>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var operation = callInfo.Arg<Func<CancellationToken, Task>>();
                var cancellationToken = callInfo.Arg<CancellationToken>();
                return operation(cancellationToken);
            });

        sut = new CancelEventReservationFacade(
            eventReservationRepository,
            eventRepository,
            unitOfWork,
            NullLogger<CancelEventReservationFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenReservationNotFound_ThrowsReservationNotFoundExceptionAndDoesNotSave()
    {
        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();

        eventReservationRepository
            .TryCancelActiveReservationAsync(eventId, userId, Arg.Any<DateTimeOffset>(), ct)
            .Returns(false);

        var ex = await Should.ThrowAsync<ReservationNotFoundException>(() => sut.HandleAsync(eventId, userId, ct));

        ex.Message.ShouldContain(eventId.ToString());
        ex.Message.ShouldContain(userId.ToString());
        await unitOfWork.Received(1).AcquireAdvisoryLockAsync(ReservationLockScope, eventId, ct);
        await eventRepository.DidNotReceive().TryFreeCapacityAsync(Arg.Any<Guid>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenActiveReservationExists_CancelsReservationFreesSeatAndSaves()
    {
        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();

        eventReservationRepository
            .TryCancelActiveReservationAsync(eventId, userId, Arg.Any<DateTimeOffset>(), ct)
            .Returns(true);
        eventReservationRepository.CountActiveByEventIdAsync(eventId, ct).Returns(3);
        eventRepository.TryFreeCapacityAsync(eventId, 3, ct).Returns(true);

        await sut.HandleAsync(eventId, userId, ct);

        await unitOfWork.Received(1).AcquireAdvisoryLockAsync(ReservationLockScope, eventId, ct);
        await eventReservationRepository.Received(1)
            .TryCancelActiveReservationAsync(eventId, userId, Arg.Any<DateTimeOffset>(), ct);
        await eventReservationRepository.Received(1).CountActiveByEventIdAsync(eventId, ct);
        await eventRepository.Received(1).TryFreeCapacityAsync(eventId, 3, ct);
        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }

    [Fact]
    public async Task HandleAsync_WhenSeatCannotBeFreed_ThrowsConflictExceptionAndDoesNotSave()
    {
        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var ct = new CancellationToken();

        eventReservationRepository
            .TryCancelActiveReservationAsync(eventId, userId, Arg.Any<DateTimeOffset>(), ct)
            .Returns(true);
        eventReservationRepository.CountActiveByEventIdAsync(eventId, ct).Returns(3);
        eventRepository.TryFreeCapacityAsync(eventId, 3, ct).Returns(false);

        var ex = await Should.ThrowAsync<ConflictException>(() => sut.HandleAsync(eventId, userId, ct));

        ex.Message.ShouldContain("Failed to free the reserved seat for this event");
        ex.Message.ShouldContain(eventId.ToString());
        await unitOfWork.Received(1).AcquireAdvisoryLockAsync(ReservationLockScope, eventId, ct);
        await eventReservationRepository.Received(1).CountActiveByEventIdAsync(eventId, ct);
        await eventRepository.Received(1).TryFreeCapacityAsync(eventId, 3, ct);
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
