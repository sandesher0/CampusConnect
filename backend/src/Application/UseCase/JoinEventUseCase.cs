using Application.Ports;
using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;
using Modules.Communities.Ports;
using Modules.Events.Domain;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;

namespace Application.UseCase;

public class JoinEventUseCase : IJoinEventUseCase
{
    private readonly IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade;
    private readonly IGetEventFacade getEventFacade;
    private readonly ICheckCommunityMembershipFacade checkCommunityMembershipFacade;
    private readonly ICheckEventReservationStatusFacade checkEventReservationStatusFacade;
    private readonly ICreateEventReservationFacade createEventReservationFacade;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<JoinEventUseCase> logger;

    public JoinEventUseCase(
        IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade,
        IGetEventFacade getEventFacade,
        ICheckCommunityMembershipFacade checkCommunityMembershipFacade,
        ICheckEventReservationStatusFacade checkEventReservationStatusFacade,
        ICreateEventReservationFacade createEventReservationFacade,
        IUnitOfWork unitOfWork,
        ILogger<JoinEventUseCase> logger
    )
    {
        this.getUserIdByAccountIdFacade = getUserIdByAccountIdFacade;
        this.getEventFacade = getEventFacade;
        this.checkCommunityMembershipFacade = checkCommunityMembershipFacade;
        this.checkEventReservationStatusFacade = checkEventReservationStatusFacade;
        this.createEventReservationFacade = createEventReservationFacade;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task HandleAsync(Guid accountId, Guid eventId, Guid communityId, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Join event requested. AccountId={AccountId}, EventId={EventId}, CommunityId={CommunityId}",
            accountId, eventId, communityId);

        var userId = await getUserIdByAccountIdFacade.HandleAsync(accountId, cancellationToken);
        if (userId is null)
        {
            logger.LogWarning("Join event rejected: no user found for AccountId={AccountId}", accountId);
            throw new AccountNotFoundException(accountId);
        }

        var @event = await getEventFacade.HandleAsync(eventId, cancellationToken);
        if (@event is null)
        {
            logger.LogWarning("Join event rejected: EventId={EventId} not found", eventId);
            throw new EventNotFoundException(eventId);
        }

        if (@event.CommunityId != communityId)
        {
            logger.LogWarning(
                "Join event rejected: EventId={EventId} belongs to CommunityId={EventCommunityId}, not CommunityId={CommunityId}",
                eventId, @event.CommunityId, communityId);
            throw new EventNotFoundException(eventId);
        }

        var communityMember = await checkCommunityMembershipFacade.HandleAsync(userId.Value, communityId, cancellationToken);
        if (communityMember is null)
        {
            logger.LogWarning(
                "Join event rejected: UserId={UserId} is not a member of CommunityId={CommunityId}",
                userId, communityId);
            throw new ForbiddenException("You need to be a member of the community to join the event");
        }

        var isAlreadyReserved = await checkEventReservationStatusFacade.HandleAsync(eventId, userId.Value, cancellationToken);
        if (isAlreadyReserved)
        {
            logger.LogWarning(
                "Join event rejected: UserId={UserId} already reserved EventId={EventId}",
                userId, eventId);
            throw new ConflictException("You have already reserved for the event");
        }

        var eventReservation = new EventReservation
        {
            EventId = eventId,
            CommunityId = @event.CommunityId,
            UserId = userId.Value
        };

        await createEventReservationFacade.HandleAsync(eventReservation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "User joined event. UserId={UserId}, EventId={EventId}, CommunityId={CommunityId}",
            userId, eventId, @event.CommunityId);
    }
}