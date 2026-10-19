// Ref: workflow.md §0 AI Development Guidelines | Feature: Application.UseCase
namespace Application.Tests.UseCases;

using Application.Tests.TestData;
using Application.UseCase;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Ports;
using Modules.Communities.Facades;
using Modules.Communities.Ports;
using NSubstitute;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using Shouldly;
using Xunit;

public class JoinPublicCommunityUseCaseTests
{
    private readonly IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade = Substitute.For<IGetUserIdByAccountIdFacade>();
    private readonly ICommunityMembershipEligibilityFacade eligibilityFacade = Substitute.For<ICommunityMembershipEligibilityFacade>();
    private readonly IJoinPublicCommunityFacade joinCommunityFacade = Substitute.For<IJoinPublicCommunityFacade>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly JoinPublicCommunityUseCase sut;

    public JoinPublicCommunityUseCaseTests()
    {
        sut = new JoinPublicCommunityUseCase(
            joinCommunityFacade,
            eligibilityFacade,
            getUserIdByAccountIdFacade,
            unitOfWork,
            NullLogger<JoinPublicCommunityUseCase>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenAccountNotFound_ThrowsAccountNotFoundExceptionAndDoesNotJoin()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var communityId = ApplicationTestData.DefaultCommunityId;
        var ct = new CancellationToken();
        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns((Guid?)null);

        var ex = await Should.ThrowAsync<AccountNotFoundException>(() => sut.HandleAsync(accountId, communityId, ct));

        ex.Message.ShouldContain(accountId.ToString());
        await eligibilityFacade.DidNotReceive()
            .HandleAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await joinCommunityFacade.DidNotReceive()
            .HandleAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenUserIneligible_ThrowsAndDoesNotJoinOrSave()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var communityId = ApplicationTestData.DefaultCommunityId;
        var ct = new CancellationToken();
        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);
        eligibilityFacade.HandleAsync(communityId, userId, ct)
            .Returns(Task.FromException(new InvalidOperationException("User is already a member")));

        await Should.ThrowAsync<InvalidOperationException>(() => sut.HandleAsync(accountId, communityId, ct));

        await joinCommunityFacade.DidNotReceive()
            .HandleAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenEligible_JoinsCommunityAndSaves()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var communityId = ApplicationTestData.DefaultCommunityId;
        var ct = new CancellationToken();
        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);

        await sut.HandleAsync(accountId, communityId, ct);

        await eligibilityFacade.Received(1).HandleAsync(communityId, userId, ct);
        await joinCommunityFacade.Received(1).HandleAsync(communityId, userId, ct);
        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }
}
