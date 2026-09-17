using Modules.Auth.Domain;
using SharedKernel.Entities;

namespace Infrastructure.Mapper.ToDomain;

public static class AccountToDomain
{
    public static Account ToDomain(AccountEntity entity)
    {
        return new Account
        {
            Id = entity.Id,
            Email = entity.Email,
            Username = entity.Username,
            PasswordHash = entity.PasswordHash,
            AccountVerifiedAt = entity.AccountVerifiedAt,
            RefreshToken = entity.RefreshToken
        };
    }
}