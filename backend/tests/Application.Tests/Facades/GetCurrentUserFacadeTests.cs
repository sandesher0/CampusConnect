// Ref: workflow.md §0 AI Development Guidelines | Feature: Application.Facades
namespace Application.Tests.Facades;

using Application.Facades;
using Application.Tests.TestData;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Ports;
using NSubstitute;
using SharedKernel.Entities;
using SharedKernel.Exceptions;
using Shouldly;
using Xunit;

public class GetCurrentUserFacadeTests
{
    private readonly IAccountRepository accountRepository = Substitute.For<IAccountRepository>();
    private readonly GetCurrentUserFacade sut;

    public GetCurrentUserFacadeTests()
    {
        sut = new GetCurrentUserFacade(accountRepository, NullLogger<GetCurrentUserFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenAccountNotFound_ThrowsAccountNotFoundException()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var ct = new CancellationToken();
        accountRepository.GetByIdAsync(accountId, ct).Returns((AccountEntity?)null);

        var ex = await Should.ThrowAsync<AccountNotFoundException>(() => sut.HandleAsync(accountId, ct));

        ex.Message.ShouldContain(accountId.ToString());
    }

    [Fact]
    public async Task HandleAsync_WhenAccountExists_ReturnsMappedAccountInfoWithUserDetails()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var ct = new CancellationToken();
        var accountEntity = new AccountEntity
        {
            Id = accountId,
            UserId = userId,
            Username = "johndoe",
            PasswordHash = "hash",
            AccountVerifiedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z"),
            User = new UserEntity
            {
                Id = userId,
                Email = "john@campus.edu",
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "111-222-3333",
                ProfileImageUrl = "https://campus.edu/john.png",
                CreatedAt = DateTimeOffset.Parse("2026-01-01T00:00:00Z")
            }
        };
        accountRepository.GetByIdAsync(accountId, ct).Returns(accountEntity);

        var result = await sut.HandleAsync(accountId, ct);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(accountId);
        result.UserId.ShouldBe(userId);
        result.Username.ShouldBe("johndoe");
        result.AccountVerifiedAt.ShouldBe(accountEntity.AccountVerifiedAt);
        result.User.ShouldNotBeNull();
        result.User.Id.ShouldBe(userId);
        result.User.Email.ShouldBe("john@campus.edu");
        result.User.FirstName.ShouldBe("John");
        result.User.LastName.ShouldBe("Doe");
        result.User.PhoneNumber.ShouldBe("111-222-3333");
        result.User.ProfileImageUrl.ShouldBe("https://campus.edu/john.png");
    }
}
