using SharedKernel.Domain;
using SharedKernel.Entities;

namespace Infrastructure.Mapper.ToEntity;

public static class PlanDomainToEntity
{
    public static PlanEntity ToEntity(Plan domain)
    {
        return new PlanEntity
        {
            Id = domain.Id,
            Code = domain.Code,
            Name = domain.Name,
            IsDefault = domain.IsDefault,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}
