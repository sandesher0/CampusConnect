using Application.Domain;
using Application.Ports;
using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;
using Modules.Communities.Ports;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using Application.Mapper.ToModuleDomain;

namespace Application.UseCase;

public class CreateEventUseCase : ICreateEventUseCase
{
    private readonly IGetUserIdByAccountIdFacade userIdByAccountIdFacade;
    private readonly ILogger<CreateEventUseCase> logger;
    private readonly IEnsureCanCreateEventFacade ensureCanCreateCommunityFacade;
    private readonly ICreateEventFacade createEventFacade;
    private readonly IUnitOfWork unitOfWork;

    public CreateEventUseCase(
            IGetUserIdByAccountIdFacade userIdByAccountIdFacade,
            IEnsureCanCreateEventFacade ensureCanCreateCommunityFacade,
            ILogger<CreateEventUseCase> logger,
            ICreateEventFacade createEventFacade,
            IUnitOfWork unitOfWork)
    {
        this.userIdByAccountIdFacade = userIdByAccountIdFacade;
        this.ensureCanCreateCommunityFacade = ensureCanCreateCommunityFacade;
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

        await ensureCanCreateCommunityFacade.HandleAsync(
        domain.CommunityId,
        userId.Value,
        cancellationToken);


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