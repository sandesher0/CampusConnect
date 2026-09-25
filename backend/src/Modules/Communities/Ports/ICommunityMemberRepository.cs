using SharedKernel.Interfaces;
using SharedKernel.Entities;
using Modules.Communities.Domain;
namespace Modules.Communities.Ports;

public interface ICommunityMemberRepository : IBaseRepository<CommunityMemberEntity>
{
    Task<CommunityMember?> GetByCommunityIdAndUserId(Guid communityId, Guid accountId, CancellationToken cancellationToken);
    Task<List<CommunityMemberEntity>> GetByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken);
}

