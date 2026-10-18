using Microsoft.Extensions.Logging;
using Modules.Events.Mapper.ToResponse;
using Modules.Events.Ports;
using SharedKernel.Constants;
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

    public async Task<PagedResult<MyEventReservationResponse>> HandleAsync(
        Guid userId,
        PageRequest page,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Fetching event reservations for user {UserId}", userId);

        var reservations = await eventReservationRepository
            .GetByUserIdAsync(userId, page, cancellationToken);

        logger.LogInformation(
            "Found {Count} reservations for user {UserId}",
            reservations.totalCount, userId);

        return reservations.Map(MyEventReservationMapper.ToResponse);
    }
}
