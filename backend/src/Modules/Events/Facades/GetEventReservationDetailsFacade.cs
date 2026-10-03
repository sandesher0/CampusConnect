using Microsoft.Extensions.Logging;
using Modules.Events.Mapper.ToResponse;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Response;

namespace Modules.Events.Facades;

public class GetEventReservationDetailsFacade : IGetEventReservationDetailsFacade
{
    private readonly IEventReservationRepository eventReservationRepository;
    private readonly IEventRepository eventRepository;
    private readonly ILogger<GetEventReservationDetailsFacade> logger;

    public GetEventReservationDetailsFacade(
        IEventReservationRepository eventReservationRepository,
        IEventRepository eventRepository,
        ILogger<GetEventReservationDetailsFacade> logger
    )
    {
        this.eventReservationRepository = eventReservationRepository;
        this.eventRepository = eventRepository;
        this.logger = logger;
    }

    public async Task<List<EventReservationSummaryResponse>> HandleAsync(Guid userId, Guid eventId, CancellationToken cancellationToken)
    {
        logger.LogDebug("Fetching reservation details for event {EventId} requested by user {UserId}", eventId, userId);

        var @event = await eventRepository.GetByIdAsync(eventId, cancellationToken);

        if (@event is null)
        {
            logger.LogInformation("Reservation details requested for non-existent event {EventId} by user {UserId}", eventId, userId);
            throw new EventNotFoundException(eventId);
        }

        if (@event.CreatedBy != userId)
        {
            logger.LogWarning("User {UserId} attempted to view reservations of event {EventId} owned by {OwnerId}", userId, eventId, @event.CreatedBy);
            throw new ForbiddenException("You don't have permission to view reservations for this event.");
        }

        var eventReservations = await eventReservationRepository.GetByEventIdAsync(eventId, cancellationToken);

        logger.LogInformation("Retrieved {Count} reservations for event {EventId}", eventReservations.Count, eventId);

        return eventReservations
            .Select(EventReservationSummaryMapper.ToSummaryResponse)
            .ToList();
    }
}