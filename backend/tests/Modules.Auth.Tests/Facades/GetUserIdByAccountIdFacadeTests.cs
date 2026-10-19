// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Auth.Facades
namespace Modules.Auth.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Facades;
using Modules.Auth.Ports;
using Modules.Auth.Tests.TestData;
using NSubstitute;
using Shouldly;
using Xunit;

public class GetUserIdByAccountIdFacadeTests
{
    private readonly IAccountRepository accountRepository = Substitute.For<IAccountRepository>();
    private readonly GetUserIdByAccountIdFacade sut;

    public GetUserIdByAccountIdFacadeTests()
    {
        sut = new GetUserIdByAccountIdFacade(accountRepository, NullLogger<GetUserIdByAccountIdFacade>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenAccountExists_ReturnsUserId()
    {
        var accountId = AuthTestData.DefaultAccountId;
        var expectedUserId = AuthTestData.DefaultUserId;
        var ct = new CancellationToken();
        accountRepository.GetUserIdByAccountIdAsync(accountId, ct).Returns(expectedUserId);

        var result = await sut.HandleAsync(accountId, ct);

        result.ShouldBe(expectedUserId);
        await accountRepository.Received(1).GetUserIdByAccountIdAsync(accountId, ct);
    }

    [Fact]
    public async Task HandleAsync_WhenAccountDoesNotExist_ReturnsNull()
    {
        var accountId = AuthTestData.DefaultAccountId;
        var ct = new CancellationToken();
        accountRepository.GetUserIdByAccountIdAsync(accountId, ct).Returns((Guid?)null);

        var result = await sut.HandleAsync(accountId, ct);

        result.ShouldBeNull();
        await accountRepository.Received(1).GetUserIdByAccountIdAsync(accountId, ct);
    }
}
