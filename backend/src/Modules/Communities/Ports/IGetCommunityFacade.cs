using Modules.Communities.Domain;
using SharedKernel.Constants;
using SharedKernel.Response;

namespace Communities.Ports;

public interface IGetCommunityFacade
{
    Task<CommunityDetailResponse?> HandleAsync(Guid id, PageRequest page, CancellationToken cancellationToken);
}