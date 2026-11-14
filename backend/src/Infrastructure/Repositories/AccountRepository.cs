using Microsoft.EntityFrameworkCore;
using Modules.Auth.Domain;
using Modules.Auth.Ports;
using SharedKernel.Entities;
using Infrastructure.Mapper.ToDomain;

namespace Infrastructure.Repositories;

public class AccountRepository : BaseRepository<AppDbContext, AccountEntity>, IAccountRepository
{
    private readonly AppDbContext dbContext;
    public AccountRepository(AppDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }


    public override async Task<AccountEntity?> GetByIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var account = await dbSet.AsNoTracking().Where(a => a.Id == accountId).Include(u => u.User).SingleOrDefaultAsync(cancellationToken);

        if (account is null)
        {
            return null;
        }
        return account;
    }

    public async Task<Account?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        var account = await dbSet.AsNoTracking().SingleOrDefaultAsync(a => a.Username == username);
        if (account is null)
            return null;
        return AccountEntityToDomain.ToDomain(account);
    }

    public async Task<AccountEntity?> GetByUsernameForLoginAsync(string username, CancellationToken cancellationToken = default)
    {
        return await dbSet
            .SingleOrDefaultAsync(account => account.Username == username, cancellationToken);
    }

    public async Task<Guid?> GetUserIdByAccountIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var entity = await dbSet.AsNoTracking().Where(a => a.Id == accountId).SingleOrDefaultAsync(cancellationToken);
        if (entity is null)
        {
            return null;
        }
        return entity.UserId;
    }

    public async Task AddRefreshTokenAsync(RefreshTokenEntity refreshToken, CancellationToken cancellationToken = default)
    {
        await dbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);
    }

    public async Task RemoveRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        await dbContext.RefreshTokens
            .Where(token => token.TokenHash == refreshTokenHash)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task PruneExpiredRefreshTokensAsync(Guid accountId, DateTimeOffset nowUtc, CancellationToken cancellationToken = default)
    {
        await dbContext.RefreshTokens
            .Where(token => token.AccountId == accountId && token.ExpiresAtUtc <= nowUtc)
            .ExecuteDeleteAsync(cancellationToken);
    }

}
