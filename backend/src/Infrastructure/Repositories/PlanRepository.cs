using Infrastructure.Mapper.ToDomain;
using Microsoft.EntityFrameworkCore;
using SharedKernel.Domain;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Infrastructure.Repositories;

public class PlanRepository : BaseRepository<AppDbContext, PlanEntity>, IPlanRepository
{
    public PlanRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Plan?> GetDefaultPlanAsync(CancellationToken cancellationToken)
    {
        var entity = await dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IsDefault, cancellationToken);

        return entity is not null ? PlanEntityToDomain.ToDomain(entity) : null;
    }

    public async Task<IReadOnlyList<PlanLimit>> GetLimitsByPlanIdAsync(Guid planId, CancellationToken cancellationToken)
    {
        var entities = await context.PlanLimits
            .AsNoTracking()
            .Where(pl => pl.PlanId == planId)
            .ToListAsync(cancellationToken);

        return entities.Select(PlanLimitEntityToDomain.ToDomain).ToList();
    }

    public async Task<UserPlan?> GetActiveUserPlanAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var entity = await context.UserPlans
            .AsNoTracking()
            .Where(up => up.UserId == userId &&
                         up.ValidFrom <= now &&
                         (up.ValidTo == null || up.ValidTo > now))
            .OrderByDescending(up => up.ValidFrom)
            .FirstOrDefaultAsync(cancellationToken);

        return entity is not null ? UserPlanEntityToDomain.ToDomain(entity) : null;
    }

    public async Task<UserPlan?> GetUserPlanAsync(Guid userId, CancellationToken cancellationToken)
    {
        var entity = await context.UserPlans
            .AsNoTracking()
            .OrderByDescending(up => up.ValidFrom)
            .FirstOrDefaultAsync(up => up.UserId == userId, cancellationToken);

        return entity is not null ? UserPlanEntityToDomain.ToDomain(entity) : null;
    }

    public async Task AssignDefaultPlanToUserAsync(Guid userId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var defaultPlan = await dbSet.FirstOrDefaultAsync(p => p.IsDefault, cancellationToken);
        if (defaultPlan == null)
        {
            throw new InvalidOperationException("Default plan is not configured in the system.");
        }

        var userPlan = new UserPlanEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            PlanId = defaultPlan.Id,
            ValidFrom = now,
            ValidTo = null,
            CreatedAt = now
        };

        await context.UserPlans.AddAsync(userPlan, cancellationToken);
    }
}
