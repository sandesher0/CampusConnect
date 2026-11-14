using Microsoft.Extensions.Logging;
using Modules.Communities.Ports;
using SharedKernel.Exceptions;

namespace Modules.Communities.Facades;

public class CommunityMembershipEligibilityFacade
    : ICommunityMembershipEligibilityFacade
{
    private readonly ICommunityRepository communityRepository;
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ILogger<CommunityMembershipEligibilityFacade> logger;

    public CommunityMembershipEligibilityFacade(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository communityMemberRepository,
        ILogger<CommunityMembershipEligibilityFacade> logger)
    {
        this.communityRepository = communityRepository;
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

        var community = await communityRepository.GetByIdAsync(communityId, cancellationToken);
        if (community is not null)
        {
            var memberCount = await communityMemberRepository.CountByCommunityIdAsync(communityId, cancellationToken);
            if (memberCount >= community.MaximumMembers)
            {
                logger.LogWarning(
                    "Community {CommunityId} reached member limit {MaximumMembers}. User {UserId} cannot join",
                    communityId,
                    community.MaximumMembers,
                    userId);

                throw new ConflictException(
                    $"Community {communityId} has reached its maximum member limit of {community.MaximumMembers}.");
            }
        }

        logger.LogDebug(
            "User {UserId} is eligible to join community {CommunityId}",
            userId,
            communityId);
    }
}