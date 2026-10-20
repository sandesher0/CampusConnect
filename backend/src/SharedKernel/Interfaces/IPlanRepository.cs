using SharedKernel.Domain;
using SharedKernel.Entities;

namespace SharedKernel.Interfaces;

public interface IPlanRepository : IBaseRepository<PlanEntity>
{
    Task<Plan?> GetDefaultPlanAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<PlanLimit>> GetLimitsByPlanIdAsync(Guid planId, CancellationToken cancellationToken);
    Task<UserPlan?> GetActiveUserPlanAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken);
    Task<UserPlan?> GetUserPlanAsync(Guid userId, CancellationToken cancellationToken);
    Task AssignDefaultPlanToUserAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken);
}
