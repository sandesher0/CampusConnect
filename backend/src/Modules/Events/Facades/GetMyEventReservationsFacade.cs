using Microsoft.Extensions.Logging;
using Modules.Events.Mapper.ToResponse;
using Modules.Events.Ports;
using SharedKernel.Response;

namespace Modules.Events.Facades;

public class GetMyEventReservationsFacade : IGetMyEventReservationsFacade
{
    private readonly IEventReservationRepository eventReservationRepository;
    private readonly ILogger<GetMyEventReservationsFacade> logger;

    public GetMyEventReservationsFacade(
        IEventReservationRepository eventReservationRepository,
        ILogger<GetMyEventReservationsFacade> logger)
    {
        this.eventReservationRepository = eventReservationRepository;
        this.logger = logger;
    }

    public async Task<List<MyEventReservationResponse>> HandleAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Fetching event reservations for user {UserId}", userId);

        var reservations = await eventReservationRepository
            .GetByUserIdAsync(userId, cancellationToken);

        logger.LogInformation(
            "Found {Count} reservations for user {UserId}",
            reservations.Count, userId);

        return reservations
            .Select(MyEventReservationMapper.ToResponse)
            .ToList();
    }
}
