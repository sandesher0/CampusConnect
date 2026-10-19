// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Events.Facades
namespace Modules.Events.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Events.Domain;
using Modules.Events.Facades;
using Modules.Events.Ports;
using Modules.Events.Tests.TestData;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Response;
using Shouldly;
using Xunit;

public class GetAllPublicEventFacadeTests
{
    private readonly IEventRepository eventRepository = Substitute.For<IEventRepository>();
    private readonly GetAllPublicEventFacade sut;

    public GetAllPublicEventFacadeTests()
    {
        sut = new GetAllPublicEventFacade(eventRepository, NullLogger<GetAllPublicEventFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_PassesPageRequest_ReturnsMappedEventSummaryResponses()
    {
        var page = new PageRequest { Page = 1, PageSize = 10 };
        var ct = new CancellationToken();
        var domainEvent = EventTestData.CreateEvent();
        var pagedDomain = new PagedResult<Event>(new List<Event> { domainEvent }, 1, 1, 10);
        eventRepository.GetAllPublicEventAsync(page, ct).Returns(pagedDomain);

        var result = await sut.HandleAsync(page, ct);

        result.totalCount.ShouldBe(1);
        result.page.ShouldBe(1);
        result.pageSize.ShouldBe(10);
        result.Items.Count.ShouldBe(1);
        result.Items[0].Id.ShouldBe(domainEvent.Id);
        result.Items[0].Title.ShouldBe(domainEvent.Title);
        result.Items[0].Location.ShouldBe(domainEvent.Location);
        await eventRepository.Received(1).GetAllPublicEventAsync(page, ct);
    }
}
