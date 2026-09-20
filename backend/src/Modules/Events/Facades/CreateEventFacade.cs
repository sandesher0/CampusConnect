

using Modules.Events.Ports;
using Microsoft.Extensions.Logging;
using Modules.Events.Domain;
using Modules.Mapper.ToEntity;

namespace Modules.Events.Facades;


public class CreateEventFacade : ICreateEventFacade
{
    private readonly IEventRepository repository;
    private readonly ILogger<CreateEventFacade> logger;

    public CreateEventFacade(
        IEventRepository repository,
        ILogger<CreateEventFacade> logger
    )
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task HandleAsync(Event domain, CancellationToken cancellationToken)
    {
        await repository.AddAsync(EventDomainToEntityMapper.ToEntity(domain), cancellationToken);
    }
}