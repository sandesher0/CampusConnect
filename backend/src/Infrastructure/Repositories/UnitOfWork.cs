using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;


namespace Infrastructure.Repositories;

public class UnitOfWork<TContext> : IUnitOfWork
where TContext : DbContext
{

    private readonly TContext context;

    public UnitOfWork(TContext context)
    {
        this.context = context;
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        int result = await context.SaveChangesAsync(cancellationToken);
        return result;
    }
}