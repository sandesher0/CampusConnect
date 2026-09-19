using Microsoft.EntityFrameworkCore;
using Modules.Users.Domain;
using Modules.Users.Ports;
using SharedKernel.Entities;

namespace Infrastructure.Repositories;

public class UserRegistrationRepository : BaseRepository<AppDbContext, UserEntity>, IUserRegistrationRepository
{
    private readonly AppDbContext dbContext;
    public UserRegistrationRepository(AppDbContext dbContext) : base(dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var userEntity = await dbSet.AsNoTracking().SingleOrDefaultAsync(u => u.Email == email);
        if (userEntity is null)
            return null;
            
        return new User 
        {
            Id = userEntity.Id,
            Email = userEntity.Email,
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
            PhoneNumber = userEntity.PhoneNumber,
            ProfileImageUrl = userEntity.ProfileImageUrl,
        };
    }
}
