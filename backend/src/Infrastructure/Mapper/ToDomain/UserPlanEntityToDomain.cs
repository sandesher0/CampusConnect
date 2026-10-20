using SharedKernel.Domain;
using SharedKernel.Entities;

namespace Infrastructure.Mapper.ToDomain;

public static class UserPlanEntityToDomain
{
    public static UserPlan ToDomain(UserPlanEntity entity)
    {
        return new UserPlan
        {
            Id = entity.Id,
            UserId = entity.UserId,
            PlanId = entity.PlanId,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo
        };
    }
}
