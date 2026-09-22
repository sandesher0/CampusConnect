using Infrastructure.Mapper.ToDomain;
using Microsoft.EntityFrameworkCore;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Entities;

namespace Infrastructure.Repositories;


public class CommunityRepository : BaseRepository<AppDbContext, CommunityEntity>, ICommunityRepository
{
    public CommunityRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<Community>?> GetAllPublicCommunitiesAsync(CancellationToken cancellationToken)
    {
        return await dbSet
        .AsNoTracking()
        .Where(x => x.CommunityType == CommunityType.Public)
        .Select(x => CommunityEntityToDomain.ToDomain(x))
        .ToListAsync(cancellationToken);
    }
}