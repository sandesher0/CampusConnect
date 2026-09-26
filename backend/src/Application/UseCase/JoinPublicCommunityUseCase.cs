using Application.Ports;
using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;
using Modules.Communities.Facades;
using Modules.Communities.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;

namespace Application.UseCase;

public class JoinPublicCommunityUseCase : IJoinPublicCommunityUseCase
{
    private readonly IJoinPublicCommunityFacade joinPublicCommunityFacade;
    private readonly ICommunityMembershipEligibilityFacade communityMembershipEligibilityFacade;
    private readonly IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<JoinPublicCommunityUseCase> logger;

    public JoinPublicCommunityUseCase(
        IJoinPublicCommunityFacade joinPublicCommunityFacade,
        ICommunityMembershipEligibilityFacade communityMembershipEligibilityFacade,
        IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade,
        IUnitOfWork unitOfWork,
        ILogger<JoinPublicCommunityUseCase> logger)
    {
        this.joinPublicCommunityFacade = joinPublicCommunityFacade;
        this.communityMembershipEligibilityFacade = communityMembershipEligibilityFacade;
        this.getUserIdByAccountIdFacade = getUserIdByAccountIdFacade;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task HandleAsync(
        Guid accountId,
        Guid communityId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Starting join community request for account {AccountId} and community {CommunityId}",
            accountId,
            communityId);

        var userId = await getUserIdByAccountIdFacade.HandleAsync(
            accountId,
            cancellationToken);

        if (userId is null)
        {
            logger.LogWarning(
                "Unable to join community. User not found for account {AccountId}",
                accountId);

            throw new AccountNotFoundException(accountId);
        }

        logger.LogDebug(
            "Resolved account {AccountId} to user {UserId}",
            accountId,
            userId.Value);

        await communityMembershipEligibilityFacade.HandleAsync(
            communityId,
            userId.Value,
            cancellationToken);

        logger.LogDebug(
            "User {UserId} is eligible to join community {CommunityId}",
            userId.Value,
            communityId);

        await joinPublicCommunityFacade.HandleAsync(
            communityId,
            userId.Value,
            cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "User {UserId} successfully joined community {CommunityId}",
            userId.Value,
            communityId);
    }
}