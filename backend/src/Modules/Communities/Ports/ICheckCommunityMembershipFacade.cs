using Modules.Communities.Domain;

namespace Modules.Communities.Ports;

public interface ICheckCommunityMembershipFacade
{
    Task<CommunityMember?> HandleAsync(Guid userId, Guid communityId, CancellationToken cancellationToken);
}