// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Events.Facades
namespace Modules.Events.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Events.Facades;
using Modules.Events.Ports;
using Modules.Events.Tests.TestData;
using NSubstitute;
using SharedKernel.Entities;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using Shouldly;
using Xunit;

public class CancelEventReservationFacadeTests
{
    private readonly IEventReservationRepository eventReservationRepository = Substitute.For<IEventReservationRepository>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CancelEventReservationFacade sut;

    public CancelEventReservationFacadeTests()
    {
        sut = new CancelEventReservationFacade(
            eventReservationRepository,
            unitOfWork,
            NullLogger<CancelEventReservationFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenReservationNotFound_ThrowsReservationNotFoundExceptionAndDoesNotSave()
    {
        var eventId = EventTestData.DefaultEventId;
        var userId = EventTestData.DefaultUserId;
        var ct = new CancellationToken();
        eventReservationRepository.GetActiveReservationAsync(eventId, userId, ct).Returns((EventReservationEntity?)null);

        var ex = await Should.ThrowAsync<ReservationNotFoundException>(() => sut.HandleAsync(eventId, userId, ct));

        ex.Message.ShouldContain(eventId.ToString());
        ex.Message.ShouldContain(userId.ToString());
        await eventReservationRepository.DidNotReceive().UpdateAsync(Arg.Any<EventReservationEntity>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenActiveReservationExists_SetsDeletedAndModifiedTimestampsAndSaves()
    {
        var eventId = EventTestData.DefaultEventId;
        var userId = EventTestData.DefaultUserId;
        var reservation = EventTestData.CreateEventReservationEntity(eventId: eventId, userId: userId);
        var ct = new CancellationToken();
        eventReservationRepository.GetActiveReservationAsync(eventId, userId, ct).Returns(reservation);

        await sut.HandleAsync(eventId, userId, ct);

        reservation.DeletedAt.ShouldNotBeNull();
        reservation.ModifiedAt.ShouldNotBeNull();
        await eventReservationRepository.Received(1).UpdateAsync(reservation, ct);
        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }
}
