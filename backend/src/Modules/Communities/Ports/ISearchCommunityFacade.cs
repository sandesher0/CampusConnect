using Modules.Communities.Domain;
using SharedKernel.Constants;

namespace Modules.Communities.Ports;

public interface ISearchCommunityFacade
{
    Task<PagedResult<Community>> HandleAsync(string searchKeyWord, PageRequest page, CancellationToken cancellationToken);
}