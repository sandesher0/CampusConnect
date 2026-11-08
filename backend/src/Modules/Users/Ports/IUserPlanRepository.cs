using SharedKernel.Domain;
using SharedKernel.Entities;

namespace Modules.Users.Ports;

public interface IUserPlanRepository
{
    Task<UserPlan?> GetActiveUserPlanAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken);
}