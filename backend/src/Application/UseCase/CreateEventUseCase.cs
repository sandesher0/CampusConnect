using Application.Domain;
using Application.Ports;
using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;
using Modules.Communities.Ports;
using Modules.Events.Policies;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using Application.Mapper.ToModuleDomain;

namespace Application.UseCase;

public class CreateEventUseCase : ICreateEventUseCase
{
    private readonly IGetUserIdByAccountIdFacade userIdByAccountIdFacade;
    private readonly ILogger<CreateEventUseCase> logger;
    private readonly IEnsureCanCreateEventFacade ensureCanCreateEventFacade;
    private readonly IEventCreationPolicy eventCreationPolicy;
    private readonly ICreateEventFacade createEventFacade;
    private readonly IUnitOfWork unitOfWork;

    public CreateEventUseCase(
            IGetUserIdByAccountIdFacade userIdByAccountIdFacade,
            IEnsureCanCreateEventFacade ensureCanCreateEventFacade,
            IEventCreationPolicy eventCreationPolicy,
            ILogger<CreateEventUseCase> logger,
            ICreateEventFacade createEventFacade,
            IUnitOfWork unitOfWork)
    {
        this.userIdByAccountIdFacade = userIdByAccountIdFacade;
        this.ensureCanCreateEventFacade = ensureCanCreateEventFacade;
        this.eventCreationPolicy = eventCreationPolicy;
        this.logger = logger;
        this.createEventFacade = createEventFacade;
        this.unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(
     Event domain,
     Guid accountId,
     CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating event for account {AccountId} in community {CommunityId}",
            accountId,
            domain.CommunityId);

        Guid? userId = await userIdByAccountIdFacade.HandleAsync(
            accountId,
            cancellationToken);

        if (userId is null)
        {
            logger.LogWarning(
                "Event creation failed because account {AccountId} was not found",
                accountId);

            throw new AccountNotFoundException(accountId);
        }

        await ensureCanCreateEventFacade.HandleAsync(
            domain.CommunityId,
            userId.Value,
            cancellationToken);

        var policyResult = await eventCreationPolicy.EvaluateAsync(userId.Value, cancellationToken);
        if (!policyResult.Succeeded)
        {
            throw new PlanLimitExceededException(policyResult.LimitKey!, policyResult.Limit!.Value);
        }

        await createEventFacade.HandleAsync(EventToEventsDomainMapper.ToDomain(domain, domain.CommunityId, userId.Value), cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Event {EventId} created successfully by user {UserId} " +
            "in community {CommunityId}",
            domain.Id,
            userId.Value,
            domain.CommunityId);
    }
}