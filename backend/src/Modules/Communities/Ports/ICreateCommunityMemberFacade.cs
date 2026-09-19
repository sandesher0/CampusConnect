using Modules.Communities.Domain;

namespace Modules.Communities.Ports;

public interface ICreateCommunityMemberFacade
{
    Task HandleAsync(CommunityMember communityMember, CancellationToken cancellationToken);
}
