using SharedKernel.Entities;
using Modules.Communities.Domain;

namespace Modules.Communities.Mapper.ToEntities;


public static class CommunityMemberDomainToEntityMapper
{
    public static CommunityMemberEntity ToEntity(CommunityMember domain)
    {
        return new CommunityMemberEntity
        {
            Id = Guid.CreateVersion7(),
            CommunityId = domain.CommunityId,
            UserId = domain.UserId,
            MemberType = domain.MemberType,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}