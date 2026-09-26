using Microsoft.Extensions.Logging;
using Modules.Communities.Domain;
using Modules.Communities.Mapper.ToEntities;
using Modules.Communities.Ports;
using SharedKernel.Constants;
using SharedKernel.Exceptions;

namespace Modules.Communities.Facades;

public class JoinPublicCommunityFacade : IJoinPublicCommunityFacade
{
    private readonly ICommunityRepository communityRepository;
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ILogger<JoinPublicCommunityFacade> logger;

    public JoinPublicCommunityFacade(
        ICommunityRepository communityRepository,
        ICommunityMemberRepository communityMemberRepository,
        ILogger<JoinPublicCommunityFacade> logger)
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
        logger.LogDebug(
            "Attempting to add user {UserId} to community {CommunityId}",
            userId,
            communityId);

        var community = await communityRepository.GetByIdAsync(
            communityId,
            cancellationToken);

        if (community is null)
        {
            logger.LogWarning(
                "Community {CommunityId} was not found while user {UserId} attempted to join",
                communityId,
                userId);

            throw new CommunityNotFoundException(communityId);
        }

        var communityMember = new CommunityMember
        {
            CommunityId = communityId,
            UserId = userId,
            MemberType = MemberType.ClubMember
        };

        await communityMemberRepository.AddAsync(
            CommunityMemberDomainToEntityMapper.ToEntity(communityMember),
            cancellationToken);

        logger.LogDebug(
            "Created community membership for user {UserId} in community {CommunityId}",
            userId,
            communityId);
    }
}