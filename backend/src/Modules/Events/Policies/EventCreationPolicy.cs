using Microsoft.Extensions.Logging;
using Modules.Events.Ports;
using SharedKernel.Constants;
using SharedKernel.Interfaces;
using SharedKernel.Models;

namespace Modules.Events.Policies;

public class EventCreationPolicy : IEventCreationPolicy
{
    private readonly IEventRepository eventRepository;
    private readonly ILimitProvider limitProvider;
    private readonly TimeProvider timeProvider;
    private readonly ILogger<EventCreationPolicy> logger;

    public EventCreationPolicy(
        IEventRepository eventRepository,
        ILimitProvider limitProvider,
        TimeProvider timeProvider,
        ILogger<EventCreationPolicy> logger)
    {
        this.eventRepository = eventRepository;
        this.limitProvider = limitProvider;
        this.timeProvider = timeProvider;
        this.logger = logger;
    }

    public async Task<PolicyResult> EvaluateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // 1. Check total created first (including soft-deleted)
        var totalLimit = await limitProvider.GetLimitAsync(userId, LimitKeys.EventsMaxTotalCreated, cancellationToken);
        if (!totalLimit.IsUnlimited)
        {
            var totalCount = await eventRepository.CountTotalCreatedByUserAsync(userId, cancellationToken);
            if (totalCount >= totalLimit.Value!.Value)
            {
                logger.LogWarning("User {UserId} reached total event creation limit. Current={Current}, Limit={Limit}",
                    userId, totalCount, totalLimit.Value.Value);

                return PolicyResult.Failure(
                    $"You have reached the maximum total limit of {totalLimit.Value.Value} events created on your current plan.",
                    LimitKeys.EventsMaxTotalCreated,
                    totalLimit.Value.Value);
            }
        }

        // 2. Check active created second (not soft-deleted and EventEndDate in the future)
        var activeLimit = await limitProvider.GetLimitAsync(userId, LimitKeys.EventsMaxActiveCreated, cancellationToken);
        if (!activeLimit.IsUnlimited)
        {
            var now = timeProvider.GetUtcNow();
            var activeCount = await eventRepository.CountActiveCreatedByUserAsync(userId, now, cancellationToken);
            if (activeCount >= activeLimit.Value!.Value)
            {
                logger.LogWarning("User {UserId} reached active event creation limit. Current={Current}, Limit={Limit}",
                    userId, activeCount, activeLimit.Value.Value);

                return PolicyResult.Failure(
                    $"You have reached the maximum active limit of {activeLimit.Value.Value} events created on your current plan.",
                    LimitKeys.EventsMaxActiveCreated,
                    activeLimit.Value.Value);
            }
        }

        return PolicyResult.Success();
    }
}
