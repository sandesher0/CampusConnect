using SharedKernel.Domain;
using SharedKernel.Entities;

namespace Infrastructure.Mapper.ToEntity;

public static class PlanLimitDomainToEntity
{
    public static PlanLimitEntity ToEntity(PlanLimit domain)
    {
        return new PlanLimitEntity
        {
            Id = domain.Id,
            PlanId = domain.PlanId,
            LimitKey = domain.LimitKey,
            Value = domain.Value,
            UpdatedAt = DateTimeOffset.UtcNow
        };
    }
}
