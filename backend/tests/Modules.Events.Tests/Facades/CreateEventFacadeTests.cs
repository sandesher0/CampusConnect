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

public class CreateEventFacadeTests
{
    private readonly IEventRepository repository = Substitute.For<IEventRepository>();
    private readonly CreateEventFacade sut;

    public CreateEventFacadeTests()
    {
        sut = new CreateEventFacade(repository, NullLogger<CreateEventFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_ValidEvent_AddsMappedEntityToRepository()
    {
        var @event = EventTestData.CreateEvent();
        var ct = new CancellationToken();

        await sut.HandleAsync(@event, ct);

        await repository.Received(1).AddAsync(
            Arg.Is<EventEntity>(e =>
                e.Title == @event.Title &&
                e.Description == @event.Description &&
                e.Location == @event.Location &&
                e.CommunityId == @event.CommunityId &&
                e.CreatedBy == @event.CreatedBy),
            ct);
    }
}
