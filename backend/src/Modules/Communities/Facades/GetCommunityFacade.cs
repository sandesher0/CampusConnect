using Communities.Ports;
using Microsoft.Extensions.Logging;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Constants;
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
        PageRequest page,
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

        var pagedMembers = await communityMemberRepository
            .GetByCommunityIdAsync(communityId, page, cancellationToken);

        logger.LogInformation(
            "Found {MemberCount} members for CommunityId: {CommunityId}",
            pagedMembers.totalCount,
            communityId);

        var response = new CommunityDetailResponse
        {
            CommunityId = community.Id,
            CommunityName = community.CommunityName,
            CommunityType = community.CommunityType,
            CreatedBy = community.CreatedBy,
            CommunityMembers = pagedMembers.Map(member => new CommunityMemberResponse
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
        };

        logger.LogInformation(
            "Successfully retrieved community details. CommunityId: {CommunityId}",
            communityId);

        return response;
    }
}
