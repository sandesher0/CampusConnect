// Ref: workflow.md §0 AI Development Guidelines | Feature: Modules.Communities.Facades
namespace Modules.Communities.Tests.Facades;

using Microsoft.Extensions.Logging.Abstractions;
using Modules.Communities.Domain;
using Modules.Communities.Facades;
using Modules.Communities.Ports;
using Modules.Communities.Tests.TestData;
using NSubstitute;
using SharedKernel.Constants;
using SharedKernel.Entities;
using SharedKernel.Interfaces;
using Shouldly;
using Xunit;

public class CreateCommunityFacadeTests
{
    private readonly ICommunityRepository communityRepository = Substitute.For<ICommunityRepository>();
    private readonly ICommunityMemberRepository communityMemberRepository = Substitute.For<ICommunityMemberRepository>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateCommunityFacade sut;

    public CreateCommunityFacadeTests()
    {
        sut = new CreateCommunityFacade(
            communityRepository,
            communityMemberRepository,
            NullLogger<CreateCommunityFacade>.Instance,
            unitOfWork);
    }

    [Fact]
    public async Task HandleAsync_ValidCommunity_AddsCommunityAndOfficerMemberAndSaves()
    {
        var community = CommunityTestData.CreateCommunity();
        var ct = new CancellationToken();

        await sut.HandleAsync(community, ct);

        await communityRepository.Received(1).AddAsync(
            Arg.Is<CommunityEntity>(c =>
                c.CommunityName == community.CommunityName &&
                c.CommunityType == community.CommunityType &&
                c.CreatedBy == community.CreatedBy &&
                c.Status == CommunityStatusType.Active),
            ct);

        await communityMemberRepository.Received(1).AddAsync(
            Arg.Is<CommunityMemberEntity>(m =>
                m.UserId == community.CreatedBy &&
                m.MemberType == MemberType.ClubOfficer),
            ct);

        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }
}
