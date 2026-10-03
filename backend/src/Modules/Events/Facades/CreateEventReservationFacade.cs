using Microsoft.Extensions.Logging;
using Modules.Events.Domain;
using Modules.Events.Mapper.ToEntity;
using Modules.Events.Ports;

namespace Modules.Events.Facades;

public class CreateEventReservationFacade : ICreateEventReservationFacade
{
    private readonly IEventReservationRepository eventReservationRepository;
    private readonly ILogger<CreateEventReservationFacade> logger;

    public CreateEventReservationFacade(
        IEventReservationRepository eventReservationRepository,
        ILogger<CreateEventReservationFacade> logger
    )
    {
        this.eventReservationRepository = eventReservationRepository;
        this.logger = logger;
    }

    public async Task HandleAsync(EventReservation domain, CancellationToken cancellationToken)
    {
        var entity = EventReservationDomainToEntity.ToEntity(domain);
        await eventReservationRepository.AddAsync(entity, cancellationToken);
        logger.LogDebug(
            "Reservation staged: ReservationId={ReservationId}, EventId={EventId}, UserId={UserId}",
            entity.Id, entity.EventId, entity.UserId);
    }
}