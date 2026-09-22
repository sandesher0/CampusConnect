using Modules.Communities.Domain;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Modules.Communities.Ports;
public interface ICommunityRepository : IBaseRepository<CommunityEntity>
{
    Task<List<Community>?> GetAllPublicCommunitiesAsync(CancellationToken cancellationToken);
}