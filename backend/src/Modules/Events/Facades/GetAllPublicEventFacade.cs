
using Microsoft.Extensions.Logging;
using Modules.Events.Domain;
using Modules.Events.Mapper.ToResponse;
using Modules.Events.Ports;
using SharedKernel.Constants;
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

    public async Task<PagedResult<EventSummaryResponse>> HandleAsync(PageRequest page, CancellationToken cancellationToken)
    {
        var pagedEvents = await eventRepository.GetAllPublicEventAsync(page, cancellationToken);

        return pagedEvents.Map(EventDomainToSummaryResponse.ToResponse);
    }
}
