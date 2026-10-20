using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using SharedKernel.Models;

namespace Application.Services;

public class LimitProvider : ILimitProvider
{
    private readonly IPlanRepository planRepository;
    private readonly TimeProvider timeProvider;
    private readonly ILogger<LimitProvider> logger;

    public LimitProvider(
        IPlanRepository planRepository,
        TimeProvider timeProvider,
        ILogger<LimitProvider> logger)
    {
        this.planRepository = planRepository;
        this.timeProvider = timeProvider;
        this.logger = logger;
    }

    public async Task<Limit> GetLimitAsync(Guid userId, string limitKey, CancellationToken cancellationToken = default)
    {
        var limits = await GetAllLimitsAsync(userId, cancellationToken);
        if (limits.TryGetValue(limitKey, out var limit))
        {
            return limit;
        }

        logger.LogError("Limit key '{LimitKey}' missing from plan and default plan for UserId={UserId}", limitKey, userId);
        throw new InvalidOperationException($"Configuration error: Limit key '{limitKey}' has no value on the plan or default plan.");
    }

    public async Task<IReadOnlyDictionary<string, Limit>> GetAllLimitsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var defaultPlan = await planRepository.GetDefaultPlanAsync(cancellationToken);
        if (defaultPlan is null)
        {
            logger.LogError("Configuration error: No default plan configured in the database.");
            throw new InvalidOperationException("Configuration error: No default plan configured in the database.");
        }

        var defaultLimitsList = await planRepository.GetLimitsByPlanIdAsync(defaultPlan.Id, cancellationToken);
        var defaultLimitsDict = defaultLimitsList.ToDictionary(l => l.LimitKey, l => new Limit(l.Value));

        var now = timeProvider.GetUtcNow();
        var activeUserPlan = await planRepository.GetActiveUserPlanAsync(userId, now, cancellationToken);

        if (activeUserPlan is not null && activeUserPlan.PlanId != defaultPlan.Id)
        {
            var userPlanLimitsList = await planRepository.GetLimitsByPlanIdAsync(activeUserPlan.PlanId, cancellationToken);
            var userPlanLimitsDict = userPlanLimitsList.ToDictionary(l => l.LimitKey, l => new Limit(l.Value));
            var resolvedLimits = new Dictionary<string, Limit>(defaultLimitsDict);
            foreach (var kvp in userPlanLimitsDict)
            {
                resolvedLimits[kvp.Key] = kvp.Value;
            }
            return resolvedLimits;
        }

        return defaultLimitsDict;
    }
}
