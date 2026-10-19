// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Events.Facades
namespace Modules.Events.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Events.Ports;
using Modules.Events.Tests.TestData;
using NSubstitute;
using SharedKernel.Entities;
using Shouldly;
using Xunit;

public class CheckEventReservationStatusFacadeTests
{
    private readonly IEventReservationRepository eventReservationRepository = Substitute.For<IEventReservationRepository>();
    private readonly CheckEventReservationStatusFacade sut;

    public CheckEventReservationStatusFacadeTests()
    {
        sut = new CheckEventReservationStatusFacade(
            eventReservationRepository,
            NullLogger<CheckEventReservationStatusFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenReservationExists_ReturnsTrue()
    {
        var eventId = EventTestData.DefaultEventId;
        var userId = EventTestData.DefaultUserId;
        var ct = new CancellationToken();
        var entity = EventTestData.CreateEventReservationEntity(eventId: eventId, userId: userId);
        eventReservationRepository.CheckEventReservation(eventId, userId, ct).Returns(entity);

        var result = await sut.HandleAsync(eventId, userId, ct);

        result.ShouldBeTrue();
        await eventReservationRepository.Received(1).CheckEventReservation(eventId, userId, ct);
    }

    [Fact]
    public async Task HandleAsync_WhenReservationDoesNotExist_ReturnsFalse()
    {
        var eventId = EventTestData.DefaultEventId;
        var userId = EventTestData.DefaultUserId;
        var ct = new CancellationToken();
        eventReservationRepository.CheckEventReservation(eventId, userId, ct).Returns((EventReservationEntity?)null);

        var result = await sut.HandleAsync(eventId, userId, ct);

        result.ShouldBeFalse();
        await eventReservationRepository.Received(1).CheckEventReservation(eventId, userId, ct);
    }
}
