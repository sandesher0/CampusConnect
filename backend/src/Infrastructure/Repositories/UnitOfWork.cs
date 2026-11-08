
using System.IO.Hashing;
using System.Text;
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

    public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await operation(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task AcquireAdvisoryLockAsync(string scope, Guid resourceId, CancellationToken cancellationToken)
    {
        if (context.Database.CurrentTransaction is null)
        {
            throw new InvalidOperationException("An active database transaction is required before acquiring a transaction-scoped advisory lock.");
        }
        var (key1, key2) = ComputeLockKeys(scope, resourceId);
       await context.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT pg_advisory_xact_lock({key1}, {key2})",
            cancellationToken);
    }

    private static (int key1, int key2) ComputeLockKeys(string scope, Guid resourceId)
    {
        var scopeBytes = Encoding.UTF8.GetBytes(scope);
        int key1 = BitConverter.ToInt32(XxHash32.Hash(scopeBytes));

        Span<byte> guidBytes = stackalloc byte[16];
        resourceId.TryWriteBytes(guidBytes);
        int key2 = BitConverter.ToInt32(guidBytes[..4])
                 ^ BitConverter.ToInt32(guidBytes.Slice(4, 4))
                 ^ BitConverter.ToInt32(guidBytes.Slice(8, 4))
                 ^ BitConverter.ToInt32(guidBytes.Slice(12, 4));
        return (key1, key2);

    }
}