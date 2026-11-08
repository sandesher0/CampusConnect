using Infrastructure.Mapper.ToDomain;
using Microsoft.EntityFrameworkCore;
using Modules.Users.Ports;
using SharedKernel.Domain;
using SharedKernel.Entities;
using SharedKernel.Interfaces;

namespace Infrastructure.Repositories;

public class PlanLimitRepository : BaseRepository<AppDbContext, PlanLimitEntity>, IPlanLimitRepository
{
    public PlanLimitRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<List<PlanLimit>> GetPlanLimitsAsync(
    Guid planId,
    CancellationToken cancellationToken)
    {
        var planLimits = await dbSet
            .AsNoTracking()
            .Where(p => p.PlanId == planId)
            .ToListAsync(cancellationToken);

        return planLimits
            .Select(PlanLimitEntityToDomain.ToDomain)
            .ToList();
    }
}
