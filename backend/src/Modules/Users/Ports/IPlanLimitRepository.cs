using SharedKernel.Domain;

namespace Modules.Users.Ports;

public interface IPlanLimitRepository
{
    Task<List<PlanLimit>> GetPlanLimitsAsync(Guid planId, CancellationToken cancellationToken);
}