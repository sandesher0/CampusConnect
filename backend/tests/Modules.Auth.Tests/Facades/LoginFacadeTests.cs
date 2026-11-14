// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Auth.Facades
namespace Modules.Auth.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Domain;
using Modules.Auth.Facades;
using Modules.Auth.Ports;
using Modules.Auth.Tests.TestData;
using SharedKernel.Entities;
using SharedKernel.Interfaces;
using NSubstitute;
using SharedKernel.Exceptions;
using Shouldly;
using Xunit;

public class LoginFacadeTests
{
    private readonly IAccountRepository accountRepository = Substitute.For<IAccountRepository>();
    private readonly IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly ITokenGenerator tokenGenerator = Substitute.For<ITokenGenerator>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly LoginFacade sut;

    public LoginFacadeTests()
    {
        sut = new LoginFacade(
            accountRepository,
            NullLogger<LoginFacade>.Instance,
            passwordHasher,
            tokenGenerator,
            unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_WhenAccountNotFound_ThrowsInvalidCredentialsException()
    {
        var login = new Login { Username = "unknown_user", Password = "Password123!" };
        var ct = new CancellationToken();
        accountRepository.GetByUsernameForLoginAsync(login.Username, ct).Returns((AccountEntity?)null);

        await Should.ThrowAsync<InvalidCredentialsException>(() => sut.HandleAsync(login, ct));

        await passwordHasher.Received(1).ComparePassword(login.Password, Arg.Any<string>());
        tokenGenerator.DidNotReceive().GenerateJwtToken(Arg.Any<Account>());
        tokenGenerator.DidNotReceive().GenerateRefreshToken();
    }

    [Fact]
    public async Task HandleAsync_WhenPasswordDoesNotMatch_ThrowsInvalidCredentialsException()
    {
        var login = new Login { Username = "valid_user", Password = "WrongPassword" };
        var account = AuthTestData.CreateAccountEntity(username: login.Username, passwordHash: "real_hash");
        var ct = new CancellationToken();
        accountRepository.GetByUsernameForLoginAsync(login.Username, ct).Returns(account);
        passwordHasher.ComparePassword(login.Password, account.PasswordHash).Returns(false);

        await Should.ThrowAsync<InvalidCredentialsException>(() => sut.HandleAsync(login, ct));

        tokenGenerator.DidNotReceive().GenerateJwtToken(Arg.Any<Account>());
        tokenGenerator.DidNotReceive().GenerateRefreshToken();
    }

    [Fact]
    public async Task HandleAsync_WhenCredentialsValid_GeneratesAndReturnsTokensAndPersistsRefreshToken()
    {
        var login = new Login { Username = "valid_user", Password = "CorrectPassword123!" };
        var accountEntity = AuthTestData.CreateAccountEntity(username: login.Username, passwordHash: "real_hash");
        var ct = new CancellationToken();
        accountRepository.GetByUsernameForLoginAsync(login.Username, ct).Returns(accountEntity);
        passwordHasher.ComparePassword(login.Password, accountEntity.PasswordHash).Returns(true);
        tokenGenerator.GenerateJwtToken(Arg.Is<Account>(a =>
            a.Id == accountEntity.Id &&
            a.UserId == accountEntity.UserId &&
            a.Username == accountEntity.Username &&
            a.PasswordHash == accountEntity.PasswordHash)).Returns("mock_jwt_token_value");
        tokenGenerator.GenerateRefreshToken().Returns("mock_refresh_token_value");

        var tokens = await sut.HandleAsync(login, ct);

        tokens.AccessToken.ShouldBe("mock_jwt_token_value");
        tokens.RefreshToken.ShouldBe("mock_refresh_token_value");
        tokenGenerator.Received(1).GenerateJwtToken(Arg.Any<Account>());
        tokenGenerator.Received(1).GenerateRefreshToken();
        await accountRepository.Received(1).AddRefreshTokenAsync(
            Arg.Is<RefreshTokenEntity>(token =>
                token.AccountId == accountEntity.Id &&
                token.TokenHash != "mock_refresh_token_value"),
            ct);
        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }
}
