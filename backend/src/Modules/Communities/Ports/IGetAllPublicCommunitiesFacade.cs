using Modules.Communities.Domain;
using SharedKernel.Constants;

namespace Modules.Communities.Ports;

public interface IGetAllPublicCommunitiesFacade
{
    Task<PagedResult<Community>> HandleAsync(PageRequest page, CancellationToken cancellationToken);
}