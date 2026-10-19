// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Auth.Facades
namespace Modules.Auth.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Domain;
using Modules.Auth.Facades;
using Modules.Auth.Ports;
using Modules.Auth.Tests.TestData;
using NSubstitute;
using SharedKernel.Entities;
using SharedKernel.Exceptions;
using Shouldly;
using Xunit;

public class AccountRegistrationFacadeTests
{
    private readonly IAccountRepository accountRepository = Substitute.For<IAccountRepository>();
    private readonly AccountRegistrationFacade sut;

    public AccountRegistrationFacadeTests()
    {
        sut = new AccountRegistrationFacade(accountRepository, NullLogger<AccountRegistrationFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenUsernameAlreadyTaken_ThrowsUsernameAlreadyTakenExceptionAndDoesNotAdd()
    {
        var account = AuthTestData.CreateAccount(username: "duplicate_user");
        var ct = new CancellationToken();
        accountRepository.GetByUsernameAsync(account.Username, ct)
            .Returns(AuthTestData.CreateAccount(username: "duplicate_user"));

        var ex = await Should.ThrowAsync<UsernameAlreadyTakenException>(() => sut.HandleAsync(account, ct));

        ex.Message.ShouldContain(account.Username);
        await accountRepository.DidNotReceive().AddAsync(Arg.Any<AccountEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenUsernameAvailable_AddsMappedAccountEntity()
    {
        var account = AuthTestData.CreateAccount(username: "new_unique_user");
        var ct = new CancellationToken();
        accountRepository.GetByUsernameAsync(account.Username, ct).Returns((Account?)null);

        await sut.HandleAsync(account, ct);

        await accountRepository.Received(1).AddAsync(
            Arg.Is<AccountEntity>(e =>
                e.Id == account.Id &&
                e.UserId == account.UserId &&
                e.Username == account.Username &&
                e.PasswordHash == account.PasswordHash),
            ct);
    }
}
