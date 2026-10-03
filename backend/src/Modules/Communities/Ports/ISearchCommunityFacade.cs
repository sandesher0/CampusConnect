using Modules.Communities.Domain;

namespace Modules.Communities.Ports;

public interface ISearchCommunityFacade
{
    Task<List<Community>> HandleAsync(string searchKeyWord, CancellationToken cancellationToken);
}