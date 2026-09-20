
using Application.Domain;
using Application.Ports;
using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;
using Modules.Communities.Ports;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Constants;
using SharedKernel.Interfaces;
using Application.Mapper.ToModuleDomain;

namespace Application.UseCase;

public class CreateEventUseCase : ICreateEventUseCase
{
    private readonly IEventRepository repository;
    private readonly IAccountRepository accountRepository;
    private readonly ICommunityRepository communityRepository;
    private readonly ILogger<CreateEventUseCase> logger;
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ICreateEventFacade createEventFacade;
    private readonly IUnitOfWork unitOfWork;

    public CreateEventUseCase(
            IEventRepository repository,
            IAccountRepository accountRepository,
            ICommunityRepository communityRepository,
            ICommunityMemberRepository communityMemberRepository,
            ILogger<CreateEventUseCase> logger,
            ICreateEventFacade createEventFacade,
            IUnitOfWork unitOfWork)
    {
        this.repository = repository;
        this.accountRepository = accountRepository;
        this.communityRepository = communityRepository;
        this.communityMemberRepository = communityMemberRepository;
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

        Guid? userId = await accountRepository.GetUserIdByAccountIdAsync(
            accountId,
            cancellationToken);

        if (userId is null)
        {
            logger.LogWarning(
                "Event creation failed because account {AccountId} was not found",
                accountId);

            throw new AccountNotFoundException(accountId);
        }

        var communityMember =
            await communityMemberRepository.GetByCommunityIdAndUserId(
                domain.CommunityId,
                userId.Value,
                cancellationToken);

        if (communityMember is null)
        {
            logger.LogWarning(
                "Event creation unauthorized for account {AccountId}: " +
                "user {UserId} is not a member of community {CommunityId}",
                accountId,
                userId.Value,
                domain.CommunityId);

            throw new UnauthorizedAccessException();
        }

        if (communityMember.MemberType != MemberType.ClubOfficer)
        {
            logger.LogWarning(
                "Event creation unauthorized for account {AccountId}: " +
                "user {UserId} is not the owner of community {CommunityId}. MemberType: {MemberType}",
                accountId,
                userId.Value,
                domain.CommunityId,
                communityMember.MemberType);

            throw new UnauthorizedAccessException();
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