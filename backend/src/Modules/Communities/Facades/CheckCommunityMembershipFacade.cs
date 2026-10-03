using Microsoft.Extensions.Logging;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Exceptions;

namespace Modules.Communities.Facades;

public class CheckCommunityMembershipFacade : ICheckCommunityMembershipFacade
{
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ILogger<CheckCommunityMembershipFacade> logger;

    public CheckCommunityMembershipFacade(
        ICommunityMemberRepository communityMemberRepository,
        ILogger<CheckCommunityMembershipFacade> logger
    )
    {
        this.communityMemberRepository = communityMemberRepository;
        this.logger = logger;
    }

    public async Task<CommunityMember?> HandleAsync(Guid userId, Guid communityId, CancellationToken cancellationToken)
    {
        var member = await communityMemberRepository.GetByCommunityIdAndUserId(communityId, userId, cancellationToken);
        logger.LogDebug(
            "Membership check: UserId={UserId}, CommunityId={CommunityId}, IsMember={IsMember}",
            userId, communityId, member is not null);
        return member;

    }
}