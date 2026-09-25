using Communities.Ports;
using Microsoft.Extensions.Logging;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Response;

namespace Modules.Communities.Facades;

public class GetCommunityFacade : IGetCommunityFacade
{
    private readonly ICommunityRepository communityRepository;
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ILogger<GetCommunityFacade> logger;

    public GetCommunityFacade(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository communityMemberRepository,
        ILogger<GetCommunityFacade> logger)
    {
        this.communityRepository = communityRepository;
        this.communityMemberRepository = communityMemberRepository;
        this.logger = logger;
    }

    public async Task<CommunityDetailResponse?> HandleAsync(
        Guid communityId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Getting community details for CommunityId: {CommunityId}",
            communityId);

        var community = await communityRepository
            .GetByIdAsync(communityId, cancellationToken);

        if (community is null)
        {
            logger.LogWarning(
                "Community not found. CommunityId: {CommunityId}",
                communityId);

            throw new CommunityNotFoundException(communityId);
        }

        var communityMembers = await communityMemberRepository
            .GetByCommunityIdAsync(communityId, cancellationToken);

        logger.LogInformation(
            "Found {MemberCount} members for CommunityId: {CommunityId}",
            communityMembers.Count,
            communityId);

        var response = new CommunityDetailResponse
        {
            CommunityId = community.Id,
            CommunityName = community.CommunityName,
            CommunityType = community.CommunityType,
            CreatedBy = community.CreatedBy,

            CommunityMembers = communityMembers
        .Select(member => new CommunityMemberResponse
        {
            MemberId = member.Id,
            UserId = member.UserId,
            CommunityId = member.CommunityId,
            MemberType = member.MemberType,
            CreatedAt = member.CreatedAt,

            User = new CommunityUserResponse
            {
                UserId = member.User.Id,
                Email = member.User.Email,
                FirstName = member.User.FirstName,
                LastName = member.User.LastName,
                ProfileImageUrl = member.User.ProfileImageUrl
            }
        })
        .ToList()
        };

        logger.LogInformation(
            "Successfully retrieved community details. CommunityId: {CommunityId}",
            communityId);

        return response;
    }
}
