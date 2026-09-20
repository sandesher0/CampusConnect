using Modules.Communities.Ports;
using SharedKernel.Entities;

namespace Infrastructure.Repositories;


public class CommunityRepository : BaseRepository<AppDbContext, CommunityEntity>, ICommunityRepository
{
    public CommunityRepository(AppDbContext context) : base(context)
    {
    }
}