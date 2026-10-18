using Modules.Communities.Domain;
using SharedKernel.Constants;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Modules.Communities.Ports;

public interface ICommunityRepository : IBaseRepository<CommunityEntity>
{
    Task<PagedResult<Community>> GetAllPublicCommunitiesAsync(PageRequest page, CancellationToken cancellationToken);
    Task<PagedResult<Community>> GetMyCommunitiesAsync(Guid userId, PageRequest page, CancellationToken cancellationToken);
    Task<PagedResult<Community>> SearchCommunityAsync(string searchKeyWord, PageRequest page, CancellationToken cancellationToken);
}