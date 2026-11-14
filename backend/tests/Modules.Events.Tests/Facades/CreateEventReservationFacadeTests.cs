// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Events.Facades
namespace Modules.Events.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Events.Domain;
using Modules.Events.Facades;
using Modules.Events.Ports;
using Modules.Events.Tests.TestData;
using NSubstitute;
using SharedKernel.Entities;
using Shouldly;
using Xunit;

public class CreateEventReservationFacadeTests
{
    private readonly IEventReservationRepository eventReservationRepository = Substitute.For<IEventReservationRepository>();
    private readonly IEventRepository eventRepository = Substitute.For<IEventRepository>();
    private readonly CreateEventReservationFacade sut;

    public CreateEventReservationFacadeTests()
    {
        sut = new CreateEventReservationFacade(
            eventReservationRepository,
            eventRepository,
            NullLogger<CreateEventReservationFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ValidReservation_AddsMappedEntityToRepository()
    {
        var reservation = EventTestData.CreateEventReservation();
        var ct = new CancellationToken();
        eventReservationRepository.CountActiveByEventIdAsync(reservation.EventId, ct)
            .Returns(0);
        eventRepository.TryReserveCapacityAsync(reservation.EventId, 0, ct)
            .Returns(true);

        await sut.HandleAsync(reservation, ct);

        await eventReservationRepository.Received(1).CountActiveByEventIdAsync(reservation.EventId, ct);
        await eventRepository.Received(1).TryReserveCapacityAsync(reservation.EventId, 0, ct);
        await eventReservationRepository.Received(1).AddAsync(
            Arg.Is<EventReservationEntity>(e =>
                e.EventId == reservation.EventId &&
                e.CommunityId == reservation.CommunityId &&
                e.UserId == reservation.UserId),
            ct);
    }
}
