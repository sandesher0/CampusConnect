using SharedKernel.Domain;
using SharedKernel.Entities;

namespace Infrastructure.Mapper.ToEntity;

public static class UserPlanDomainToEntity
{
    public static UserPlanEntity ToEntity(UserPlan domain)
    {
        return new UserPlanEntity
        {
            Id = domain.Id,
            UserId = domain.UserId,
            PlanId = domain.PlanId,
            ValidFrom = domain.ValidFrom,
            ValidTo = domain.ValidTo,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
