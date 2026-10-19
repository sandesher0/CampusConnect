// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Events.Facades
namespace Modules.Events.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Events.Facades;
using Modules.Events.Ports;
using Modules.Events.Tests.TestData;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Entities;
using Shouldly;
using Xunit;

public class GetMyEventReservationsFacadeTests
{
    private readonly IEventReservationRepository eventReservationRepository = Substitute.For<IEventReservationRepository>();
    private readonly GetMyEventReservationsFacade sut;

    public GetMyEventReservationsFacadeTests()
    {
        sut = new GetMyEventReservationsFacade(
            eventReservationRepository,
            NullLogger<GetMyEventReservationsFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_PassesUserIdAndPageRequest_ReturnsMappedReservations()
    {
        var userId = EventTestData.DefaultUserId;
        var page = new PageRequest { Page = 1, PageSize = 10 };
        var ct = new CancellationToken();
        var reservationEntity = EventTestData.CreateEventReservationEntity(userId: userId);
        var pagedEntities = new PagedResult<EventReservationEntity>(new List<EventReservationEntity> { reservationEntity }, 1, 1, 10);
        eventReservationRepository.GetByUserIdAsync(userId, page, ct).Returns(pagedEntities);

        var result = await sut.HandleAsync(userId, page, ct);

        result.totalCount.ShouldBe(1);
        result.Items.Count.ShouldBe(1);
        result.Items[0].ReservationId.ShouldBe(reservationEntity.Id);
        result.Items[0].EventId.ShouldBe(reservationEntity.EventId);
        result.Items[0].Title.ShouldBe(reservationEntity.Event.Title);
        result.Items[0].Location.ShouldBe(reservationEntity.Event.Location);
        await eventReservationRepository.Received(1).GetByUserIdAsync(userId, page, ct);
    }
}
