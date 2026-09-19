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
        return AccountToDomain.ToDomain(account);
    }

}