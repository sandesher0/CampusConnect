
using Modules.Communities.Domain;
using SharedKernel.Constants;

namespace Modules.Communities.Ports;

public interface IGetMyCommunitiesFacade
{
    Task<PagedResult<Community>> HandleAsync(Guid userId, PageRequest page, CancellationToken cancellationToken);
}