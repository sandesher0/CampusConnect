using Modules.Communities.Domain;

namespace Modules.Communities.Ports;

public interface IGetAllPublicCommunitiesFacade
{
    Task<List<Community>?> HandleAsync(CancellationToken cancellationToken);
}