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
using SharedKernel.Interfaces;
using Shouldly;
using Xunit;

public class ChangePasswordFacadeTests
{
    private readonly IAccountRepository accountRepository = Substitute.For<IAccountRepository>();
    private readonly IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ChangePasswordFacade sut;

    public ChangePasswordFacadeTests()
    {
        sut = new ChangePasswordFacade(
            accountRepository,
            NullLogger<ChangePasswordFacade>.Instance,
            unitOfWork,
            passwordHasher);
    }

    [Fact]
    public async Task HandleAsync_WhenAccountNotFound_ThrowsAccountNotFoundExceptionAndDoesNotSave()
    {
        var accountId = AuthTestData.DefaultAccountId;
        var request = new ChangePassword { OldPassword = "old", NewPassword = "new" };
        var ct = new CancellationToken();
        accountRepository.GetByIdAsync(accountId, ct).Returns((AccountEntity?)null);

        var ex = await Should.ThrowAsync<AccountNotFoundException>(() => sut.HandleAsync(accountId, request, ct));

        ex.Message.ShouldContain(accountId.ToString());
        await passwordHasher.DidNotReceive().ComparePassword(Arg.Any<string>(), Arg.Any<string>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenOldPasswordDoesNotMatch_ThrowsInvalidCredentialsExceptionAndDoesNotSave()
    {
        var accountId = AuthTestData.DefaultAccountId;
        var request = new ChangePassword { OldPassword = "WrongOldPassword", NewPassword = "NewPassword123!" };
        var ct = new CancellationToken();
        var entity = AuthTestData.CreateAccountEntity(id: accountId, passwordHash: "existing_hash");
        accountRepository.GetByIdAsync(accountId, ct).Returns(entity);
        passwordHasher.ComparePassword(request.OldPassword, entity.PasswordHash).Returns(false);

        await Should.ThrowAsync<InvalidCredentialsException>(() => sut.HandleAsync(accountId, request, ct));

        await passwordHasher.DidNotReceive().HashPassword(Arg.Any<string>());
        await accountRepository.DidNotReceive().UpdateAsync(Arg.Any<AccountEntity>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenOldPasswordMatches_HashesNewPasswordUpdatesAndSaves()
    {
        var accountId = AuthTestData.DefaultAccountId;
        var request = new ChangePassword { OldPassword = "ValidOldPassword", NewPassword = "NewPassword123!" };
        var ct = new CancellationToken();
        var entity = AuthTestData.CreateAccountEntity(id: accountId, passwordHash: "existing_hash");
        accountRepository.GetByIdAsync(accountId, ct).Returns(entity);
        passwordHasher.ComparePassword(request.OldPassword, entity.PasswordHash).Returns(true);
        passwordHasher.HashPassword(request.NewPassword).Returns("new_hashed_password");

        await sut.HandleAsync(accountId, request, ct);

        entity.PasswordHash.ShouldBe("new_hashed_password");
        await accountRepository.Received(1).UpdateAsync(entity);
        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }
}
