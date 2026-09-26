
namespace Modules.Communities.Ports;

public interface IJoinPublicCommunityFacade
{
    Task HandleAsync(Guid communityId, Guid userId, CancellationToken cancellationToken);
}