namespace Modules.Communities.Mapper.ToEntities;

using Modules.Communities.Domain;
using SharedKernel.Entities;
using SharedKernel.Constants;


public static class CommunityDomainToEntityMapper
{
    public static CommunityEntity ToEntity(Community domain)
    {
        return new CommunityEntity
        {
            Id = Guid.CreateVersion7(),
            CommunityName = domain.CommunityName,
            CommunityType = domain.CommunityType,
            Status = CommunityStatusType.Active,
            CreatedBy = domain.CreatedBy,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}