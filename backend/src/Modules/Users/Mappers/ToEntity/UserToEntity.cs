using Modules.Users.Domain;
using SharedKernel.Entities;

namespace Modules.Users.Mapper.ToEntity;

public static class UserDomainToEntity
{
    public static UserEntity ToEntity(User domain)
    {
        return new UserEntity
        {
            Id = Guid.CreateVersion7(),
            Email = domain.Email,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            PhoneNumber = domain.PhoneNumber,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }
}