using Microsoft.EntityFrameworkCore;
using Modules.Auth.Domain;
using Modules.Auth.Ports;
using SharedKernel.Entities;
using Infrastructure.Mapper.ToDomain;

namespace Infrastructure.Repositories;

public class UserRegistrationRepository : BaseRepository<AppDbContext, AccountEntity>, IUserRegistrationRepository
{
    private readonly AppDbContext dbContext;
    public UserRegistrationRepository(AppDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Account?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var account = await dbSet.AsNoTracking().SingleOrDefaultAsync(a => a.Email == email);
        if (account is null)
            return null;
        return AccountToDomain.ToDomain(account);

    }
}