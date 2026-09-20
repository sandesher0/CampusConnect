using Modules.Communities.Domain;

namespace Modules.Communities.Ports;

public interface ICreateCommunityFacade
{
    Task HandleAsync(Community community, CancellationToken cancellationToken);
}