
using Microsoft.Extensions.Logging;
using Modules.Events.Domain;
using Modules.Events.Mapper.ToResponse;
using Modules.Events.Ports;
using SharedKernel.Response;

namespace Modules.Events.Facades;

public class GetAllPublicEventFacade : IGetAllPublicEventFacade
{
    private readonly IEventRepository eventRepository;
    private readonly ILogger<GetAllPublicEventFacade> logger;

    public GetAllPublicEventFacade(
        IEventRepository eventRepository,
        ILogger<GetAllPublicEventFacade> logger)
    {
        this.eventRepository = eventRepository;
        this.logger = logger;
    }

    public async Task<List<EventResponse>> HandleAsync(CancellationToken cancellationToken)
    {
        var events = await eventRepository.GetAllPublicEventAsync(cancellationToken);

        return events
            .Select(EventDomainToResponse.ToResponse)
            .ToList();
    }

}