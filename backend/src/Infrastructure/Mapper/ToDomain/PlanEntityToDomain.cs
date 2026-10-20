using SharedKernel.Domain;
using SharedKernel.Entities;

namespace Infrastructure.Mapper.ToDomain;

public static class PlanEntityToDomain
{
    public static Plan ToDomain(PlanEntity entity)
    {
        return new Plan
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            IsDefault = entity.IsDefault
        };
    }
}
