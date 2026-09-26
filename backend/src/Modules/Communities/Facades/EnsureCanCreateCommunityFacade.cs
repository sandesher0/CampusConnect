using Microsoft.Extensions.Logging;
using Modules.Communities.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Constants;

namespace Modules.Communities.Facades;

public class EnsureCanCreateEventFacade : IEnsureCanCreateEventFacade
{
    private readonly ICommunityRepository communityRepository;
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ILogger<IEnsureCanCreateEventFacade> logger;

    public EnsureCanCreateEventFacade(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository communityMemberRepository,
        ILogger<IEnsureCanCreateEventFacade> logger)
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
            throw new CommunityNotFoundException(communityId);
        }

        var communityMember =
            await communityMemberRepository.GetByCommunityIdAndUserId(
                communityId,
                userId,
                cancellationToken);

        if (communityMember is null ||
            communityMember.MemberType != MemberType.ClubOfficer)
        {
            throw new UnauthorizedAccessException();
        }
    }
}