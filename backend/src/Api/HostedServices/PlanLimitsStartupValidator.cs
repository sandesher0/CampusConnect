using SharedKernel.Constants;
using SharedKernel.Interfaces;

namespace Api.HostedServices;

public class PlanLimitsStartupValidator : IHostedService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<PlanLimitsStartupValidator> logger;

    public PlanLimitsStartupValidator(
        IServiceProvider serviceProvider,
        ILogger<PlanLimitsStartupValidator> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var planRepository = scope.ServiceProvider.GetRequiredService<IPlanRepository>();

        logger.LogInformation("Validating default plan limits on startup...");

        var defaultPlan = await planRepository.GetDefaultPlanAsync(cancellationToken);
        if (defaultPlan == null)
        {
            throw new InvalidOperationException("Fatal configuration error: No default plan found in database.");
        }

        var limits = await planRepository.GetLimitsByPlanIdAsync(defaultPlan.Id, cancellationToken);
        var limitsDict = limits.ToDictionary(l => l.LimitKey);

        string[] requiredKeys =
        [
            LimitKeys.EventsMaxTotalCreated,
            LimitKeys.EventsMaxActiveCreated,
            LimitKeys.CommunitiesMaxCreated,
            LimitKeys.EventsMaxCapacity
        ];

        foreach (var key in requiredKeys)
        {
            if (!limitsDict.TryGetValue(key, out var limitRecord) || limitRecord.Value is null)
            {
                throw new InvalidOperationException($"Fatal configuration error: Required limit '{key}' has no configured value on default plan '{defaultPlan.Code}'.");
            }
        }

        logger.LogInformation("Default plan limits startup validation succeeded.");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
