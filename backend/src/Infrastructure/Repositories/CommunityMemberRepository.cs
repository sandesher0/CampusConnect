using Infrastructure.Mapper.ToDomain;
using Microsoft.EntityFrameworkCore;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Entities;

namespace Infrastructure.Repositories;

public class CommunityMemberRepository : BaseRepository<AppDbContext, CommunityMemberEntity>, ICommunityMemberRepository
{
    public CommunityMemberRepository(AppDbContext context) : base(context)
    {

    }

    public async Task<CommunityMember?> GetByCommunityIdAndUserId(Guid communityId, Guid userId, CancellationToken cancellationToken)
    {
        var communityMember = await dbSet.
        AsNoTracking().
        Where(cm => cm.CommunityId == communityId && cm.UserId == userId).
        FirstOrDefaultAsync(cancellationToken);
        if (communityMember is null)
        {
            return null;
        }
        return CommunityMemberToDomain.ToDomain(communityMember);
    }

    public async Task<List<CommunityMemberEntity>> GetByCommunityIdAsync(
        Guid communityId,
        CancellationToken cancellationToken)
    {
        return await dbSet
            .AsNoTracking()
            .Where(x =>
                x.CommunityId == communityId &&
                x.DeletedAt == null)
            .Include(x => x.User)
            .ToListAsync(cancellationToken);
    }


}