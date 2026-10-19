// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Auth.Facades
namespace Modules.Auth.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Domain;
using Modules.Auth.Facades;
using Modules.Auth.Ports;
using Modules.Auth.Tests.TestData;
using NSubstitute;
using SharedKernel.Exceptions;
using Shouldly;
using Xunit;

public class LoginFacadeTests
{
    private readonly IAccountRepository accountRepository = Substitute.For<IAccountRepository>();
    private readonly IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenGenerator tokenGenerator = Substitute.For<ITokenGenerator>();
    private readonly LoginFacade sut;

    public LoginFacadeTests()
    {
        sut = new LoginFacade(
            accountRepository,
            NullLogger<LoginFacade>.Instance,
            passwordHasher,
            tokenGenerator);
    }

    [Fact]
    public async Task HandleAsync_WhenAccountNotFound_ThrowsInvalidCredentialsException()
    {
        var login = new Login { Username = "unknown_user", Password = "Password123!" };
        var ct = new CancellationToken();
        accountRepository.GetByUsernameAsync(login.Username, ct).Returns((Account?)null);

        await Should.ThrowAsync<InvalidCredentialsException>(() => sut.HandleAsync(login, ct));

        await passwordHasher.DidNotReceive().ComparePassword(Arg.Any<string>(), Arg.Any<string>());
        tokenGenerator.DidNotReceive().GenerateJwtToken(Arg.Any<Account>());
    }

    [Fact]
    public async Task HandleAsync_WhenPasswordDoesNotMatch_ThrowsInvalidCredentialsException()
    {
        var login = new Login { Username = "valid_user", Password = "WrongPassword" };
        var account = AuthTestData.CreateAccount(username: login.Username, passwordHash: "real_hash");
        var ct = new CancellationToken();
        accountRepository.GetByUsernameAsync(login.Username, ct).Returns(account);
        passwordHasher.ComparePassword(login.Password, account.PasswordHash).Returns(false);

        await Should.ThrowAsync<InvalidCredentialsException>(() => sut.HandleAsync(login, ct));

        tokenGenerator.DidNotReceive().GenerateJwtToken(Arg.Any<Account>());
    }

    [Fact]
    public async Task HandleAsync_WhenCredentialsValid_GeneratesAndReturnsAccessToken()
    {
        var login = new Login { Username = "valid_user", Password = "CorrectPassword123!" };
        var account = AuthTestData.CreateAccount(username: login.Username, passwordHash: "real_hash");
        var ct = new CancellationToken();
        accountRepository.GetByUsernameAsync(login.Username, ct).Returns(account);
        passwordHasher.ComparePassword(login.Password, account.PasswordHash).Returns(true);
        tokenGenerator.GenerateJwtToken(account).Returns("mock_jwt_token_value");

        var token = await sut.HandleAsync(login, ct);

        token.ShouldBe("mock_jwt_token_value");
        tokenGenerator.Received(1).GenerateJwtToken(account);
    }
}
