// Ref: workflow.md §0 AI Development Guidelines | Feature: Application.UseCase
namespace Application.Tests.UseCases;

using Application.Tests.TestData;
using Application.UseCase;
using Microsoft.Extensions.Logging.Abstractions;
using Modules.Auth.Ports;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using Modules.Events.Domain;
using Modules.Events.Ports;
using NSubstitute;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;
using SharedKernel.Response;
using Shouldly;
using Xunit;

public class JoinEventUseCaseTests
{
    private readonly IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade = Substitute.For<IGetUserIdByAccountIdFacade>();
    private readonly IGetEventFacade getEventFacade = Substitute.For<IGetEventFacade>();
    private readonly ICheckCommunityMembershipFacade checkCommunityMembershipFacade = Substitute.For<ICheckCommunityMembershipFacade>();
    private readonly ICheckEventReservationStatusFacade checkEventReservationStatusFacade = Substitute.For<ICheckEventReservationStatusFacade>();
    private readonly ICreateEventReservationFacade createEventReservationFacade = Substitute.For<ICreateEventReservationFacade>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly JoinEventUseCase sut;

    public JoinEventUseCaseTests()
    {
        sut = new JoinEventUseCase(
            getUserIdByAccountIdFacade,
            getEventFacade,
            checkCommunityMembershipFacade,
            checkEventReservationStatusFacade,
            createEventReservationFacade,
            unitOfWork,
            NullLogger<JoinEventUseCase>.Instance);
    }

    [Fact]
    public async Task HandleAsync_WhenAccountNotFound_ThrowsAccountNotFoundException()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var eventId = ApplicationTestData.DefaultEventId;
        var communityId = ApplicationTestData.DefaultCommunityId;
        var ct = new CancellationToken();
        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns((Guid?)null);

        var ex = await Should.ThrowAsync<AccountNotFoundException>(() => sut.HandleAsync(accountId, eventId, communityId, ct));

        ex.Message.ShouldContain(accountId.ToString());
        await getEventFacade.DidNotReceive().HandleAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ThrowsEventNotFoundException()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var eventId = ApplicationTestData.DefaultEventId;
        var communityId = ApplicationTestData.DefaultCommunityId;
        var ct = new CancellationToken();
        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);
        getEventFacade.HandleAsync(eventId, ct).Returns((EventDetailResponse)null!);

        var ex = await Should.ThrowAsync<EventNotFoundException>(() => sut.HandleAsync(accountId, eventId, communityId, ct));

        ex.Message.ShouldContain(eventId.ToString());
    }

    [Fact]
    public async Task HandleAsync_WhenEventBelongsToDifferentCommunity_ThrowsEventNotFoundException()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var eventId = ApplicationTestData.DefaultEventId;
        var providedCommunityId = ApplicationTestData.DefaultCommunityId;
        var actualCommunityId = Guid.Parse("99999999-0000-0000-0000-000000000000");
        var ct = new CancellationToken();

        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);
        getEventFacade.HandleAsync(eventId, ct).Returns(new EventDetailResponse
        {
            Id = eventId,
            CommunityId = actualCommunityId,
            Title = "Event",
            Description = "Desc",
            Location = "Loc",
            CreatedBy = userId,
            EventDate = DateTimeOffset.UtcNow,
            EventEndDate = DateTimeOffset.UtcNow.AddHours(2),
            Visibility = SharedKernel.Constants.EventVisibility.Public,
            Category = SharedKernel.Constants.EventCategory.Academic,
            Organizer = new EventOrganizerUserResponse { Id = userId, Email = "test@campus.edu", FirstName = "A", LastName = "B" },
            OrganizerCommunity = new EventOrganizerCommunityResponse { CommunityId = actualCommunityId, CommunityName = "Comm", CommunityType = SharedKernel.Constants.CommunityType.Public }
        });

        var ex = await Should.ThrowAsync<EventNotFoundException>(() => sut.HandleAsync(accountId, eventId, providedCommunityId, ct));

        ex.Message.ShouldContain(eventId.ToString());
    }

    [Fact]
    public async Task HandleAsync_WhenUserNotCommunityMember_ThrowsForbiddenException()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var eventId = ApplicationTestData.DefaultEventId;
        var communityId = ApplicationTestData.DefaultCommunityId;
        var ct = new CancellationToken();

        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);
        getEventFacade.HandleAsync(eventId, ct).Returns(new EventDetailResponse
        {
            Id = eventId,
            CommunityId = communityId,
            Title = "Event",
            Description = "Desc",
            Location = "Loc",
            CreatedBy = userId,
            EventDate = DateTimeOffset.UtcNow,
            EventEndDate = DateTimeOffset.UtcNow.AddHours(2),
            Visibility = SharedKernel.Constants.EventVisibility.Public,
            Category = SharedKernel.Constants.EventCategory.Academic,
            Organizer = new EventOrganizerUserResponse { Id = userId, Email = "test@campus.edu", FirstName = "A", LastName = "B" },
            OrganizerCommunity = new EventOrganizerCommunityResponse { CommunityId = communityId, CommunityName = "Comm", CommunityType = SharedKernel.Constants.CommunityType.Public }
        });
        checkCommunityMembershipFacade.HandleAsync(userId, communityId, ct).Returns((CommunityMember?)null);

        var ex = await Should.ThrowAsync<ForbiddenException>(() => sut.HandleAsync(accountId, eventId, communityId, ct));

        ex.Message.ShouldContain("member of the community");
    }

    [Fact]
    public async Task HandleAsync_WhenAlreadyReserved_ThrowsConflictException()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var eventId = ApplicationTestData.DefaultEventId;
        var communityId = ApplicationTestData.DefaultCommunityId;
        var ct = new CancellationToken();

        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);
        getEventFacade.HandleAsync(eventId, ct).Returns(new EventDetailResponse
        {
            Id = eventId,
            CommunityId = communityId,
            Title = "Event",
            Description = "Desc",
            Location = "Loc",
            CreatedBy = userId,
            EventDate = DateTimeOffset.UtcNow,
            EventEndDate = DateTimeOffset.UtcNow.AddHours(2),
            Visibility = SharedKernel.Constants.EventVisibility.Public,
            Category = SharedKernel.Constants.EventCategory.Academic,
            Organizer = new EventOrganizerUserResponse { Id = userId, Email = "test@campus.edu", FirstName = "A", LastName = "B" },
            OrganizerCommunity = new EventOrganizerCommunityResponse { CommunityId = communityId, CommunityName = "Comm", CommunityType = SharedKernel.Constants.CommunityType.Public }
        });
        checkCommunityMembershipFacade.HandleAsync(userId, communityId, ct)
            .Returns(new CommunityMember { CommunityId = communityId, UserId = userId, MemberType = SharedKernel.Constants.MemberType.ClubMember });
        checkEventReservationStatusFacade.HandleAsync(eventId, userId, ct).Returns(true);

        var ex = await Should.ThrowAsync<ConflictException>(() => sut.HandleAsync(accountId, eventId, communityId, ct));

        ex.Message.ShouldContain("already reserved");
        await createEventReservationFacade.DidNotReceive()
            .HandleAsync(Arg.Any<EventReservation>(), Arg.Any<CancellationToken>());
        await unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_WhenAllValidationsPass_CreatesReservationAndSaves()
    {
        var accountId = ApplicationTestData.DefaultAccountId;
        var userId = ApplicationTestData.DefaultUserId;
        var eventId = ApplicationTestData.DefaultEventId;
        var communityId = ApplicationTestData.DefaultCommunityId;
        var ct = new CancellationToken();

        getUserIdByAccountIdFacade.HandleAsync(accountId, ct).Returns(userId);
        getEventFacade.HandleAsync(eventId, ct).Returns(new EventDetailResponse
        {
            Id = eventId,
            CommunityId = communityId,
            Title = "Event",
            Description = "Desc",
            Location = "Loc",
            CreatedBy = userId,
            EventDate = DateTimeOffset.UtcNow,
            EventEndDate = DateTimeOffset.UtcNow.AddHours(2),
            Visibility = SharedKernel.Constants.EventVisibility.Public,
            Category = SharedKernel.Constants.EventCategory.Academic,
            Organizer = new EventOrganizerUserResponse { Id = userId, Email = "test@campus.edu", FirstName = "A", LastName = "B" },
            OrganizerCommunity = new EventOrganizerCommunityResponse { CommunityId = communityId, CommunityName = "Comm", CommunityType = SharedKernel.Constants.CommunityType.Public }
        });
        checkCommunityMembershipFacade.HandleAsync(userId, communityId, ct)
            .Returns(new CommunityMember { CommunityId = communityId, UserId = userId, MemberType = SharedKernel.Constants.MemberType.ClubMember });
        checkEventReservationStatusFacade.HandleAsync(eventId, userId, ct).Returns(false);

        await sut.HandleAsync(accountId, eventId, communityId, ct);

        await createEventReservationFacade.Received(1).HandleAsync(
            Arg.Is<EventReservation>(r =>
                r.EventId == eventId &&
                r.CommunityId == communityId &&
                r.UserId == userId),
            ct);
        await unitOfWork.Received(1).SaveChangesAsync(ct);
    }
}
