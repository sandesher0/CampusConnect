using Microsoft.Extensions.Logging;
using Modules.Communities.Ports;
using SharedKernel.Constants;
using SharedKernel.Interfaces;
using SharedKernel.Models;

namespace Modules.Communities.Policies;

public class CommunityCreationPolicy : ICommunityCreationPolicy
{
    private readonly ICommunityRepository communityRepository;
    private readonly ILimitProvider limitProvider;
    private readonly ILogger<CommunityCreationPolicy> logger;

    public CommunityCreationPolicy(
        ICommunityRepository communityRepository,
        ILimitProvider limitProvider,
        ILogger<CommunityCreationPolicy> logger)
    {
        this.communityRepository = communityRepository;
        this.limitProvider = limitProvider;
        this.logger = logger;
    }

    public async Task<PolicyResult> EvaluateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var limit = await limitProvider.GetLimitAsync(userId, LimitKeys.CommunitiesMaxCreated, cancellationToken);
        if (limit.IsUnlimited)
        {
            return PolicyResult.Success();
        }

        var count = await communityRepository.CountCreatedByUserAsync(userId, cancellationToken);
        if (count >= limit.Value!.Value)
        {
            logger.LogWarning("User {UserId} reached community creation limit. Current={Current}, Limit={Limit}",
                userId, count, limit.Value.Value);

            return PolicyResult.Failure(
                $"You have reached the limit of {limit.Value.Value} communities for your current plan.",
                LimitKeys.CommunitiesMaxCreated,
                limit.Value.Value);
        }

        return PolicyResult.Success();
    }
}
