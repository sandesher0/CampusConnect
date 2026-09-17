using Modules.Auth.Domain;
using SharedKernel.Entities;

namespace Modules.Auth.Mapper.ToEntity;

public static class AccountDomainToEntity
{
    public static AccountEntity ToEntity(Account domain)
    {
        return new AccountEntity
        {
            Id = Guid.CreateVersion7(),
            Email = domain.Email,
            Username = domain.Username,
            PasswordHash = domain.PasswordHash,
        };
    }
}