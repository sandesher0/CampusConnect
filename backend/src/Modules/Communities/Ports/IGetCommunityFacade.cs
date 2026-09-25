using Modules.Communities.Domain;
using SharedKernel.Response;

namespace Communities.Ports;

public interface IGetCommunityFacade
{
    Task<CommunityDetailResponse?> HandleAsync(Guid id, CancellationToken cancellationToken);
}