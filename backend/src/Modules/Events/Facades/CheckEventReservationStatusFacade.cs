using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Modules.Events.Ports;

public class CheckEventReservationStatusFacade : ICheckEventReservationStatusFacade
{
    private readonly IEventReservationRepository eventReservationRepository;
    private readonly ILogger<CheckEventReservationStatusFacade> logger;


    public CheckEventReservationStatusFacade(
        IEventReservationRepository eventReservationRepository,
        ILogger<CheckEventReservationStatusFacade> logger
    )
    {
        this.eventReservationRepository = eventReservationRepository;
        this.logger = logger;
    }

    public async Task<bool> HandleAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        var reservation = await eventReservationRepository.CheckEventReservation(eventId, userId, cancellationToken);
        var exists = reservation is not null;
        logger.LogDebug(
            "Reservation check: EventId={EventId}, UserId={UserId}, Exists={Exists}",
            eventId, userId, exists);
        return exists;
    }
}