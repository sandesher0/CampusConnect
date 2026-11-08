using Microsoft.Extensions.Logging;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;

namespace Modules.Events.Facades;

public class CancelEventReservationFacade : ICancelEventReservationFacade
{
    private readonly IEventReservationRepository eventReservationRepository;
    private readonly IEventRepository eventRepository;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<CancelEventReservationFacade> logger;

    public CancelEventReservationFacade(
        IEventReservationRepository eventReservationRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork,
        ILogger<CancelEventReservationFacade> logger)
    {
        this.eventReservationRepository = eventReservationRepository;
        this.eventRepository = eventRepository;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task HandleAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to cancel reservation for EventId={EventId}, UserId={UserId}", eventId, userId);
        var cancelledAt = DateTimeOffset.UtcNow;

        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            var reservationCancelled = await eventReservationRepository.TryCancelActiveReservationAsync(
                eventId,
                userId,
                cancelledAt,
                cancellationToken);
            if (!reservationCancelled)
            {
                logger.LogWarning("Reservation not found for cancellation: EventId={EventId}, UserId={UserId}", eventId, userId);
                throw new ReservationNotFoundException(eventId, userId);
            }

            var seatFreed = await eventRepository.TryFreeCapacityAsync(eventId, cancellationToken);
            if (!seatFreed)
            {
                logger.LogError(
                    "Failed to free seat while cancelling reservation. EventId={EventId}, UserId={UserId}",
                    eventId, userId);
                throw new ConflictException($"Failed to free the reserved seat for this event: {eventId}.");
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        logger.LogInformation("Reservation cancelled successfully: EventId={EventId}, UserId={UserId}",
            eventId, userId);
    }
}
