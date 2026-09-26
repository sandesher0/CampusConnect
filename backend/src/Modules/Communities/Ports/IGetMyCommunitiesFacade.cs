
using Modules.Communities.Domain;

namespace Modules.Communities.Ports;

public interface IGetMyCommunitiesFacade
{
    Task<List<Community>> HandleAsync(Guid userId, CancellationToken cancellationToken);
}