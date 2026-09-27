using Microsoft.Extensions.Logging;
using Modules.Communities.Ports;

namespace Modules.Communities.Facades;

public class CommunityMembershipEligibilityFacade
    : ICommunityMembershipEligibilityFacade
{
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ILogger<CommunityMembershipEligibilityFacade> logger;

    public CommunityMembershipEligibilityFacade(
        ICommunityMemberRepository communityMemberRepository,
        ILogger<CommunityMembershipEligibilityFacade> logger)
    {
        this.communityMemberRepository = communityMemberRepository;
        this.logger = logger;
    }

    public async Task HandleAsync(
        Guid communityId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogDebug(
            "Checking membership eligibility for user {UserId} in community {CommunityId}",
            userId,
            communityId);

        var communityMember =
            await communityMemberRepository.GetByCommunityIdAndUserId(
                communityId,
                userId,
                cancellationToken);

        if (communityMember is not null)
        {
            logger.LogWarning(
                "User {UserId} is already a member of community {CommunityId}",
                userId,
                communityId);

            throw new InvalidOperationException(
                $"User {userId} is already a member of community {communityId}.");
        }

        logger.LogDebug(
            "User {UserId} is eligible to join community {CommunityId}",
            userId,
            communityId);
    }
}