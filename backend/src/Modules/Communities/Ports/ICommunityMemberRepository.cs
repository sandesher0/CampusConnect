using SharedKernel.Interfaces;
using SharedKernel.Entities;
using Modules.Communities.Domain;
using SharedKernel.Constants;
namespace Modules.Communities.Ports;

public interface ICommunityMemberRepository : IBaseRepository<CommunityMemberEntity>
{
    Task<CommunityMember?> GetByCommunityIdAndUserId(Guid communityId, Guid accountId, CancellationToken cancellationToken);
    Task<PagedResult<CommunityMemberEntity>> GetByCommunityIdAsync(Guid communityId, PageRequest page, CancellationToken cancellationToken);
}

