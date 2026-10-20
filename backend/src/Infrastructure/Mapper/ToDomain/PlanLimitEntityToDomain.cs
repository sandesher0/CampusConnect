using SharedKernel.Domain;
using SharedKernel.Entities;

namespace Infrastructure.Mapper.ToDomain;

public static class PlanLimitEntityToDomain
{
    public static PlanLimit ToDomain(PlanLimitEntity entity)
    {
        return new PlanLimit
        {
            Id = entity.Id,
            PlanId = entity.PlanId,
            LimitKey = entity.LimitKey,
            Value = entity.Value
        };
    }
}
