namespace Infrastructure.Repositories;

using Microsoft.EntityFrameworkCore;
using SharedKernel.Entities;
using SharedKernel.Domain;
using Infrastructure.Mapper.ToDomain;
using Modules.Users.Ports;

public class UserPlanRepository : BaseRepository<AppDbContext, UserPlanEntity>, IUserPlanRepository
{
    public UserPlanRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<UserPlan?> GetActiveUserPlanAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var entity = await dbSet.AsNoTracking()
            .Where(up => up.UserId == userId &&
                         up.ValidFrom <= now &&
                         (up.ValidTo == null || up.ValidTo > now))
            .OrderByDescending(up => up.ValidFrom)
            .FirstOrDefaultAsync(cancellationToken);

        return entity is not null ? UserPlanEntityToDomain.ToDomain(entity) : null;
    }

}