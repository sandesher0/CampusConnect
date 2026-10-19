// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Events.Facades
namespace Modules.Events.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Events.Facades;
using Modules.Events.Ports;
using Modules.Events.Tests.TestData;
using NSubstitute;
using Modules.Events.Domain;
using SharedKernel.Entities;
using SharedKernel.Exceptions;
using Shouldly;
using Xunit;

public class GetEventFacadeTests
{
    private readonly IEventRepository repository = Substitute.For<IEventRepository>();
    private readonly GetEventFacade sut;

    public GetEventFacadeTests()
    {
        sut = new GetEventFacade(repository, NullLogger<GetEventFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ThrowsEventNotFoundException()
    {
        var eventId = EventTestData.DefaultEventId;
        var ct = new CancellationToken();
        repository.GetByEventIdAsync(eventId, ct).Returns((EventEntity?)null);

        var ex = await Should.ThrowAsync<EventNotFoundException>(() => sut.HandleAsync(eventId, ct));

        ex.Message.ShouldContain(eventId.ToString());
    }

    [Fact]
    public async Task HandleAsync_WhenEventExists_ReturnsMappedEventDetailResponse()
    {
        var eventId = EventTestData.DefaultEventId;
        var ct = new CancellationToken();
        var entity = EventTestData.CreateEventEntity(id: eventId);
        repository.GetByEventIdAsync(eventId, ct).Returns(entity);

        var result = await sut.HandleAsync(eventId, ct);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(entity.Id);
        result.Title.ShouldBe(entity.Title);
        result.Organizer.Email.ShouldBe(entity.User.Email);
        result.OrganizerCommunity.CommunityName.ShouldBe(entity.Community.CommunityName);
        await repository.Received(1).GetByEventIdAsync(eventId, ct);
    }
}
