using Modules.Users.Domain;
using SharedKernel.Entities;

namespace Modules.Users.Mapper.ToEntity;

public static class UserDomainToEntity
{
    public static UserEntity ToEntity(User domain)
    {
        return new UserEntity
        {
            Id = domain.Id,
            Email = domain.Email,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            PhoneNumber = domain.PhoneNumber,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}