using Application.Ports;
using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;
using Modules.Communities.Ports;
using Modules.Events.Ports;
using SharedKernel.Constants;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using SharedKernel.Response;

namespace Application.UseCase;

public class GetUserLimitsUseCase : IGetUserLimitsUseCase
{
    private readonly IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade;
    private readonly IPlanRepository planRepository;
    private readonly ILimitProvider limitProvider;
    private readonly ICommunityRepository communityRepository;
    private readonly IEventRepository eventRepository;
    private readonly TimeProvider timeProvider;
    private readonly ILogger<GetUserLimitsUseCase> logger;

    public GetUserLimitsUseCase(
        IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade,
        IPlanRepository planRepository,
        ILimitProvider limitProvider,
        ICommunityRepository communityRepository,
        IEventRepository eventRepository,
        TimeProvider timeProvider,
        ILogger<GetUserLimitsUseCase> logger)
    {
        this.getUserIdByAccountIdFacade = getUserIdByAccountIdFacade;
        this.planRepository = planRepository;
        this.limitProvider = limitProvider;
        this.communityRepository = communityRepository;
        this.eventRepository = eventRepository;
        this.timeProvider = timeProvider;
        this.logger = logger;
    }

    public async Task<UserLimitsResponse> HandleAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var userId = await getUserIdByAccountIdFacade.HandleAsync(accountId, cancellationToken);
        if (userId is null)
        {
            throw new AccountNotFoundException(accountId);
        }

        var now = timeProvider.GetUtcNow();
        var userPlan = await planRepository.GetActiveUserPlanAsync(userId.Value, now, cancellationToken);

        var plan = userPlan != null
            ? (await planRepository.GetByIdAsync(userPlan.PlanId, cancellationToken))
            : null;

        var defaultPlan = await planRepository.GetDefaultPlanAsync(cancellationToken);
        if (defaultPlan is null)
        {
            throw new InvalidOperationException("Default plan not configured in database.");
        }

        var planCode = plan?.Code ?? defaultPlan.Code;
        var planName = plan?.Name ?? defaultPlan.Name;

        var limits = await limitProvider.GetAllLimitsAsync(userId.Value, cancellationToken);

        var communitiesCount = await communityRepository.CountCreatedByUserAsync(userId.Value, cancellationToken);
        var eventsTotalCount = await eventRepository.CountTotalCreatedByUserAsync(userId.Value, cancellationToken);
        var eventsActiveCount = await eventRepository.CountActiveCreatedByUserAsync(userId.Value, now, cancellationToken);

        var limitsDict = new Dictionary<string, LimitDetailResponse>();

        foreach (var (key, limit) in limits)
        {
            int? currentUsage = key switch
            {
                LimitKeys.CommunitiesMaxCreated => communitiesCount,
                LimitKeys.EventsMaxTotalCreated => eventsTotalCount,
                LimitKeys.EventsMaxActiveCreated => eventsActiveCount,
                _ => null
            };

            limitsDict[key] = new LimitDetailResponse(limit.Value, currentUsage, limit.IsUnlimited);
        }

        return new UserLimitsResponse(planCode, planName, limitsDict);
    }
}
