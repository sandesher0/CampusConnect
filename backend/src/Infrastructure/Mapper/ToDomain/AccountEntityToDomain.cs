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
            UserId = entity.UserId,
            Username = entity.Username,
            PasswordHash = entity.PasswordHash,
            AccountVerifiedAt = entity.AccountVerifiedAt,
            RefreshToken = entity.RefreshToken
        };
    }
}