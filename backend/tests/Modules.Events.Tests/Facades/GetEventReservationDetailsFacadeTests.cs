// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Events.Facades
namespace Modules.Events.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Events.Domain;
using Modules.Events.Facades;
using Modules.Events.Ports;
using Modules.Events.Tests.TestData;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Entities;
using SharedKernel.Exceptions;
using Shouldly;
using Xunit;

public class GetEventReservationDetailsFacadeTests
{
    private readonly IEventReservationRepository eventReservationRepository = Substitute.For<IEventReservationRepository>();
    private readonly IEventRepository eventRepository = Substitute.For<IEventRepository>();
    private readonly GetEventReservationDetailsFacade sut;

    public GetEventReservationDetailsFacadeTests()
    {
        sut = new GetEventReservationDetailsFacade(
            eventReservationRepository,
            eventRepository,
            NullLogger<GetEventReservationDetailsFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ThrowsEventNotFoundException()
    {
        var userId = EventTestData.DefaultUserId;
        var eventId = EventTestData.DefaultEventId;
        var page = new PageRequest();
        var ct = new CancellationToken();
        eventRepository.GetByIdAsync(eventId, ct).Returns((EventEntity?)null);

        var ex = await Should.ThrowAsync<EventNotFoundException>(() => sut.HandleAsync(userId, eventId, page, ct));

        ex.Message.ShouldContain(eventId.ToString());
        await eventReservationRepository.DidNotReceive()
            .GetByEventIdAsync(Arg.Any<Guid>(), Arg.Any<PageRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsNotEventCreator_ThrowsForbiddenException()
    {
        var requestingUserId = Guid.Parse("12121212-1212-1212-1212-121212121212");
        var eventCreatorId = EventTestData.DefaultUserId;
        var eventId = EventTestData.DefaultEventId;
        var page = new PageRequest();
        var ct = new CancellationToken();
        var eventEntity = EventTestData.CreateEventEntity(id: eventId, createdBy: eventCreatorId);
        eventRepository.GetByIdAsync(eventId, ct).Returns(eventEntity);

        var ex = await Should.ThrowAsync<ForbiddenException>(() => sut.HandleAsync(requestingUserId, eventId, page, ct));

        ex.Message.ShouldContain("don't have permission");
        await eventReservationRepository.DidNotReceive()
            .GetByEventIdAsync(Arg.Any<Guid>(), Arg.Any<PageRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenUserIsEventCreator_ReturnsPagedReservationSummaries()
    {
        var creatorUserId = EventTestData.DefaultUserId;
        var eventId = EventTestData.DefaultEventId;
        var page = new PageRequest { Page = 1, PageSize = 10 };
        var ct = new CancellationToken();
        var eventEntity = EventTestData.CreateEventEntity(id: eventId, createdBy: creatorUserId);
        var reservationEntity = EventTestData.CreateEventReservationEntity(eventId: eventId);
        var pagedEntities = new PagedResult<EventReservationEntity>(new List<EventReservationEntity> { reservationEntity }, 1, 1, 10);

        eventRepository.GetByIdAsync(eventId, ct).Returns(eventEntity);
        eventReservationRepository.GetByEventIdAsync(eventId, page, ct).Returns(pagedEntities);

        var result = await sut.HandleAsync(creatorUserId, eventId, page, ct);

        result.totalCount.ShouldBe(1);
        result.Items.Count.ShouldBe(1);
        result.Items[0].Id.ShouldBe(reservationEntity.Id);
        result.Items[0].UserId.ShouldBe(reservationEntity.UserId);
        result.Items[0].FirstName.ShouldBe(reservationEntity.User.FirstName);
        result.Items[0].LastName.ShouldBe(reservationEntity.User.LastName);
    }
}
