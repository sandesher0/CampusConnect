using Microsoft.Extensions.Logging;
using Modules.Communities.Ports;
using SharedKernel.Constants;
using SharedKernel.Exceptions;

namespace Modules.Communities.Facades;

public class EnsureCanCreateEventFacade : IEnsureCanCreateEventFacade
{
    private readonly ICommunityRepository communityRepository;
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ILogger<EnsureCanCreateEventFacade> logger;

    public EnsureCanCreateEventFacade(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository communityMemberRepository,
        ILogger<EnsureCanCreateEventFacade> logger)
    {
        this.communityRepository = communityRepository;
        this.communityMemberRepository = communityMemberRepository;
        this.logger = logger;
    }

    public async Task HandleAsync(
        Guid communityId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        var community = await communityRepository.GetByIdAsync(
            communityId,
            cancellationToken);

        if (community is null)
        {
            logger.LogWarning(
                "Cannot create event. Community {CommunityId} was not found.",
                communityId);

            throw new CommunityNotFoundException(communityId);
        }

        var communityMember =
            await communityMemberRepository.GetByCommunityIdAndUserId(
                communityId,
                userId,
                cancellationToken);

        if (communityMember is null)
        {
            logger.LogWarning(
                "User {UserId} attempted to create an event in community {CommunityId}, but is not a member.",
                userId,
                communityId);

            throw new UnauthorizedAccessException();
        }

        if (communityMember.MemberType != MemberType.ClubOfficer)
        {
            logger.LogWarning(
                "User {UserId} attempted to create an event in community {CommunityId} with member type {MemberType}.",
                userId,
                communityId,
                communityMember.MemberType);

            throw new UnauthorizedAccessException();
        }
    }
}
