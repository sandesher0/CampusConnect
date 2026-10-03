using Infrastructure.Mapper.ToDomain;
using Microsoft.EntityFrameworkCore;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Entities;
using SharedKernel.Constants;

namespace Infrastructure.Repositories;


public class CommunityRepository : BaseRepository<AppDbContext, CommunityEntity>, ICommunityRepository
{
    public CommunityRepository(AppDbContext context) : base(context)
    {

    }
    public async Task<List<Community>> GetAllPublicCommunitiesAsync(CancellationToken cancellationToken)
    {
        return await dbSet
        .AsNoTracking()
        .Where(x => x.CommunityType == CommunityType.Public)
        .Select(x => CommunityEntityToDomain.ToDomain(x))
        .ToListAsync(cancellationToken);
    }

    public async Task<List<Community>> GetMyCommunitiesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var communityMember = context.Set<CommunityMemberEntity>();
        return await dbSet
                .AsNoTracking()
                .Where(c => c.DeletedAt == null && (
                    c.CreatedBy == userId ||
                    communityMember.Any(m =>
                    m.CommunityId == c.Id &&
                    m.UserId == userId &&
                    m.DeletedAt == null)
                ))
                .Select(x => CommunityEntityToDomain.ToDomain(x))
                .ToListAsync(cancellationToken);
    }

    public async Task<List<Community>> SearchCommunityAsync(string searchKeyWord, CancellationToken cancellationToken)
    {
        var foundCommunities = await dbSet.
                                AsNoTracking().
                                Where(
                                    c => c.DeletedAt == null &&
                                    c.CommunityType == CommunityType.Public &&
                                    EF.Functions.ILike(c.CommunityName, $"%{searchKeyWord}%"))
                                    .Select(x => CommunityEntityToDomain.ToDomain(x)).ToListAsync(cancellationToken);

        return foundCommunities;
    }
}