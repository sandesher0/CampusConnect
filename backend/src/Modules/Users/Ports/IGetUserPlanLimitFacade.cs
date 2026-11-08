using SharedKernel.Domain;

namespace Modules.Users.Ports;

public interface IGetUserPlanLimitFacade
{
    Task<List<PlanLimit>> GetUserPlanLimitAsync(Guid userId, CancellationToken cancellationToken);
}