using Microsoft.Extensions.Logging;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;

namespace Modules.Events.Facades;

public class CancelEventReservationFacade : ICancelEventReservationFacade
{
    private readonly IEventReservationRepository eventReservationRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CancelEventReservationFacade> logger;

    public CancelEventReservationFacade(
        IEventReservationRepository eventReservationRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelEventReservationFacade> logger)
    {
        this.eventReservationRepository = eventReservationRepository;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task HandleAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to cancel reservation for EventId={EventId}, UserId={UserId}", eventId, userId);

        var reservation = await eventReservationRepository.GetActiveReservationAsync(eventId, userId, cancellationToken);
        if (reservation is null)
        {
            logger.LogWarning("Reservation not found for cancellation: EventId={EventId}, UserId={UserId}", eventId, userId);
            throw new ReservationNotFoundException(eventId, userId);
        }

        reservation.DeletedAt = DateTimeOffset.UtcNow;
        reservation.ModifiedAt = DateTimeOffset.UtcNow;

        await eventReservationRepository.UpdateAsync(reservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Reservation cancelled successfully: ReservationId={ReservationId}, EventId={EventId}, UserId={UserId}",
            reservation.Id, eventId, userId);
    }
}
