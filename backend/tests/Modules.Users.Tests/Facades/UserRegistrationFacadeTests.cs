// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Users.Facades
namespace Modules.Users.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Users.Domain;
using Modules.Users.Facades;
using Modules.Users.Ports;
using Modules.Users.Tests.TestData;
using NSubstitute;
using SharedKernel.Entities;
using SharedKernel.Exceptions;
using Shouldly;
using Xunit;

public class UserRegistrationFacadeTests
{
    private readonly IUserRegistrationRepository userRegistrationRepository = Substitute.For<IUserRegistrationRepository>();
    private readonly UserRegistrationFacade sut;

    public UserRegistrationFacadeTests()
    {
        sut = new UserRegistrationFacade(
            userRegistrationRepository,
            NullLogger<UserRegistrationFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenUserWithEmailAlreadyExists_ThrowsAccountAlreadyExistsExceptionAndDoesNotAdd()
    {
        var user = UserTestData.CreateUser(email: "duplicate@campus.edu");
        var ct = new CancellationToken();
        var existingUser = UserTestData.CreateUser(email: user.Email);
        userRegistrationRepository.FindByEmailAsync(user.Email, ct).Returns(existingUser);

        var ex = await Should.ThrowAsync<AccountAlreadyExistsException>(() => sut.HandleAsync(user, ct));

        ex.Message.ShouldContain(user.Email);
        await userRegistrationRepository.DidNotReceive().AddAsync(Arg.Any<UserEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenEmailAvailable_AddsMappedUserEntity()
    {
        var user = UserTestData.CreateUser(email: "unique@campus.edu");
        var ct = new CancellationToken();
        userRegistrationRepository.FindByEmailAsync(user.Email, ct).Returns((User?)null);

        await sut.HandleAsync(user, ct);

        await userRegistrationRepository.Received(1).AddAsync(
            Arg.Is<UserEntity>(u =>
                u.Id == user.Id &&
                u.Email == user.Email &&
                u.FirstName == user.FirstName &&
                u.LastName == user.LastName &&
                u.PhoneNumber == user.PhoneNumber),
            ct);
    }
}
