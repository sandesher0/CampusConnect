using Microsoft.Extensions.Logging;
using Modules.Users.Ports;
using SharedKernel.Domain;

namespace Modules.Users.Facades;

public class GetUserPlanLimitFacade : IGetUserPlanLimitFacade
{
    private readonly IUserPlanRepository userPlanRepository;
    private readonly IPlanLimitRepository planLimitRepository;
    private readonly ILogger<GetUserPlanLimitFacade> logger;

    public GetUserPlanLimitFacade(
        IUserPlanRepository userPlanRepository,
        IPlanLimitRepository planLimitRepository,
        ILogger<GetUserPlanLimitFacade> logger
    )
    {
        this.userPlanRepository = userPlanRepository;
        this.planLimitRepository = planLimitRepository;
        this.logger = logger;
    }

    public async Task<List<PlanLimit>> GetUserPlanLimitAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Fetching active plan for user {UserId}",
            userId);

        var userPlan = await userPlanRepository.GetActiveUserPlanAsync(
            userId,
            DateTimeOffset.UtcNow,
            cancellationToken);

        if (userPlan is null)
        {
            logger.LogWarning(
                "No active plan found for user {UserId}",
                userId);

            throw new Exception($"No active plan found for user {userId}");
        }

        logger.LogInformation(
            "Active plan {PlanId} found for user {UserId}",
            userPlan.PlanId,
            userId);

        var planLimits = await planLimitRepository.GetPlanLimitsAsync(
            userPlan.PlanId,
            cancellationToken);

        logger.LogInformation(
            "Successfully retrieved {LimitCount} plan limits for user {UserId}, PlanId {PlanId}",
            planLimits.Count,
            userId,
            userPlan.PlanId);

        return planLimits;
    }
}
