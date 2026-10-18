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
    public async Task<PagedResult<Community>> GetAllPublicCommunitiesAsync(PageRequest page, CancellationToken cancellationToken)
    {
        var pagedEntities = await dbSet
            .AsNoTracking()
            .Where(x => x.CommunityType == CommunityType.Public && x.DeletedAt == null)
            .OrderBy(x => x.CommunityName)
            .ToPagedAsync(page, cancellationToken);

        return pagedEntities.Map(CommunityEntityToDomain.ToDomain);
    }

    public async Task<PagedResult<Community>> GetMyCommunitiesAsync(Guid userId, PageRequest page, CancellationToken cancellationToken)
    {
        var communityMember = context.Set<CommunityMemberEntity>();
        var pagedEntities = await dbSet
            .AsNoTracking()
            .Where(c => c.DeletedAt == null && (
                c.CreatedBy == userId ||
                communityMember.Any(m =>
                    m.CommunityId == c.Id &&
                    m.UserId == userId &&
                    m.DeletedAt == null)
            ))
            .OrderBy(x => x.CommunityName)
            .ToPagedAsync(page, cancellationToken);

        return pagedEntities.Map(CommunityEntityToDomain.ToDomain);
    }

    public async Task<PagedResult<Community>> SearchCommunityAsync(string searchKeyWord, PageRequest page, CancellationToken cancellationToken)
    {
        var pagedEntities = await dbSet
            .AsNoTracking()
            .Where(
                c => c.DeletedAt == null &&
                c.CommunityType == CommunityType.Public &&
                EF.Functions.ILike(c.CommunityName, $"%{searchKeyWord}%"))
            .OrderBy(x => x.CommunityName)
            .ToPagedAsync(page, cancellationToken);

        return pagedEntities.Map(CommunityEntityToDomain.ToDomain);
    }
}