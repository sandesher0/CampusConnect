using SharedKernel.Entities;
using Modules.Communities.Domain;

namespace Infrastructure.Mapper.ToDomain;

public static class CommunityEntityToDomain
{
    public static Community ToDomain(CommunityEntity entity)
    {
        return new Community
        {
            Id = entity.Id,
            CommunityName = entity.CommunityName,
            CommunityType = entity.CommunityType,
            CreatedBy = entity.CreatedBy,
            Status = entity.Status
        };
    }
}