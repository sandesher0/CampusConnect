using Microsoft.Extensions.Logging;
using Modules.Events.Domain;
using Modules.Events.Mapper.ToEntity;
using Modules.Events.Ports;
using SharedKernel.Exceptions;

namespace Modules.Events.Facades;

public class CreateEventReservationFacade : ICreateEventReservationFacade
{
    private readonly IEventReservationRepository eventReservationRepository;
    private readonly IEventRepository eventRepository;
    private readonly ILogger<CreateEventReservationFacade> logger;

    public CreateEventReservationFacade(
        IEventReservationRepository eventReservationRepository,
        IEventRepository eventRepository,
        ILogger<CreateEventReservationFacade> logger
    )
    {
        this.eventReservationRepository = eventReservationRepository;
        this.eventRepository = eventRepository;
        this.logger = logger;
    }

    public async Task HandleAsync(EventReservation domain, CancellationToken cancellationToken)
    {
        var isSuccess = await eventRepository.TryReserveCapacityAsync(
    domain.EventId,
    cancellationToken);

        if (!isSuccess)
        {
            throw new ConflictException(
                $"Event {domain.EventId} is at capacity.");
        }

        var entity = EventReservationDomainToEntity.ToEntity(domain);
        await eventReservationRepository.AddAsync(entity, cancellationToken);
        logger.LogDebug(
            "Reservation staged: ReservationId={ReservationId}, EventId={EventId}, UserId={UserId}",
            entity.Id, entity.EventId, entity.UserId);
    }
}