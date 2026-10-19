// Ref: workflow.md §0 AI Development Guidelines | Feature: TestData
namespace Modules.Auth.Tests.TestData;

using Modules.Auth.Domain;
using SharedKernel.Entities;

public static class AuthTestData
{
    public static readonly Guid DefaultAccountId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid DefaultUserId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public static Account CreateAccount(
        Guid? id = null,
        Guid? userId = null,
        string username = "testuser",
        string passwordHash = "dGVzdHNhbHQ=.dGVzdGhhc2g=") =>
        new()
        {
            Id = id ?? DefaultAccountId,
            UserId = userId ?? DefaultUserId,
            Username = username,
            PasswordHash = passwordHash,
            AccountVerifiedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
        };

    public static AccountEntity CreateAccountEntity(
        Guid? id = null,
        Guid? userId = null,
        string username = "testuser",
        string passwordHash = "dGVzdHNhbHQ=.dGVzdGhhc2g=") =>
        new()
        {
            Id = id ?? DefaultAccountId,
            UserId = userId ?? DefaultUserId,
            Username = username,
            PasswordHash = passwordHash,
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
        };
}
