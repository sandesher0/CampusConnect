using Microsoft.Extensions.Logging;
using Modules.Events.Mapper.ToResponse;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Response;

namespace Modules.Events.Facades;

public class GetEventFacade : IGetEventFacade
{
    private readonly IEventRepository repository;
    private readonly ILogger<GetEventFacade> logger;

    public GetEventFacade(
        IEventRepository repository,
        ILogger<GetEventFacade> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<EventResponse> HandleAsync(Guid eventId, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(eventId, cancellationToken);

        if (entity is null)
        {
            logger.LogWarning("Event {EventId} was not found.", eventId);
            throw new EventNotFoundException(eventId);
        }
        return EventEntityToResponse.ToResponse(entity);
    }
}