// Ref: workflow.md §0 AI Development Guidelines | Feature: Application.UseCase
namespace Application.Tests.UseCases;

using Application.Ports;
using Application.Tests.TestData;
using Application.UseCase;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Domain;
using Modules.Auth.Ports;
using Modules.Users.Domain;
using Modules.Users.Ports;
using NSubstitute;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using Shouldly;
using Xunit;

public class RegisterUserUseCaseTests
{
    private readonly IAccountRegistrationFacade accountRegistrationFacade = Substitute.For<IAccountRegistrationFacade>();
    private readonly IUserRegistrationFacade userRegistrationFacade = Substitute.For<IUserRegistrationFacade>();
    private readonly IPasswordHasher passwordHasher = Substitute.For<IPasswordHasher>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly RegisterUserUseCase sut;

    public RegisterUserUseCaseTests()
    {
        sut = new RegisterUserUseCase(
            accountRegistrationFacade,
            userRegistrationFacade,
            passwordHasher,
            unitOfWork,
            NullLogger<RegisterUserUseCase>.Instance);
    }

    [Fact]
    public async Task ExecuteAsync_WhenUserRegistrationFailsWithDuplicateEmail_ThrowsAndDoesNotHashOrSave()
    {
        var registerRequest = ApplicationTestData.CreateRegisterAccount();
        var ct = new CancellationToken();
        userRegistrationFacade.HandleAsync(Arg.Any<User>(), ct)
            .Returns(Task.FromException(new AccountAlreadyExistsException(registerRequest.Email)));

        await Should.ThrowAsync<AccountAlreadyExistsException>(() => sut.ExecuteAsync(registerRequest, ct));

        await passwordHasher.DidNotReceive().HashPassword(Arg.Any<string>());
        await accountRegistrationFacade.DidNotReceive().HandleAsync(Arg.Any<Account>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_WhenAccountRegistrationFailsWithDuplicateUsername_ThrowsAndDoesNotSave()
    {
        var registerRequest = ApplicationTestData.CreateRegisterAccount();
        var ct = new CancellationToken();
        passwordHasher.HashPassword(registerRequest.Password).Returns("hashed_pass");
        accountRegistrationFacade.HandleAsync(Arg.Any<Account>(), ct)
            .Returns(Task.FromException(new UsernameAlreadyTakenException(registerRequest.Username)));

        await Should.ThrowAsync<UsernameAlreadyTakenException>(() => sut.ExecuteAsync(registerRequest, ct));

        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_RegistersUserHashesPasswordRegistersAccountAndSaves()
    {
        var registerRequest = ApplicationTestData.CreateRegisterAccount();
        var ct = new CancellationToken();
        passwordHasher.HashPassword(registerRequest.Password).Returns("hashed_secure_pass");

        await sut.ExecuteAsync(registerRequest, ct);

        await userRegistrationFacade.Received(1).HandleAsync(
            Arg.Is<User>(u =>
                u.Email == registerRequest.Email &&
                u.FirstName == registerRequest.FirstName &&
                u.LastName == registerRequest.LastName),
            ct);

        await passwordHasher.Received(1).HashPassword(registerRequest.Password);

        await accountRegistrationFacade.Received(1).HandleAsync(
            Arg.Is<Account>(a =>
                a.Username == registerRequest.Username &&
                a.PasswordHash == "hashed_secure_pass"),
            ct);

        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }
}
