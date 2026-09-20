using Modules.Communities.Domain;
using SharedKernel.Entities;

namespace Infrastructure.Mapper.ToDomain;

public static class CommunityMemberToDomain
{
    public static CommunityMember ToDomain(CommunityMemberEntity entity)
    {
        return new CommunityMember
        {
            Id = entity.Id,
            CommunityId = entity.CommunityId,
            UserId = entity.UserId,
            MemberType = entity.MemberType
        };
    }
}