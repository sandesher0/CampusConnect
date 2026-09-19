using Modules.Communities.Domain;
using Modules.Communities.Ports;
using Modules.Communities.Mapper.ToEntities;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using SharedKernel.Constants;

namespace Modules.Communities.Facades;

public class CreateCommunityFacade : ICreateCommunityFacade
{
    private readonly ICommunityRepository repository;
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ILogger<CreateCommunityFacade> logger;
    private readonly IUnitOfWork unitOfWork;

    public CreateCommunityFacade(
        ICommunityRepository repository,
        ICommunityMemberRepository communityMemberRepository,
        ILogger<CreateCommunityFacade> logger,
        IUnitOfWork unitOfWork)
    {
        this.repository = repository;
        this.communityMemberRepository = communityMemberRepository;
        this.logger = logger;
        this.unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(Community community, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Community creation started. CreatedBy={UserId}, ClubName={ClubName}, ClubType={ClubType}",
            community.CreatedBy, community.CommunityName, community.CommunityType);

        var communityEntity = CommunityDomainToEntityMapper.ToEntity(community);

        await repository.AddAsync(communityEntity, cancellationToken);

        logger.LogInformation(
            "Community entity staged for insert. CommunityId={CommunityId}, ClubName={ClubName}",
            communityEntity.Id, communityEntity.CommunityName);

        var communityMemberDomain = new CommunityMember
        {
            CommunityId = communityEntity.Id,
            MemberType = MemberType.Owner,
            UserId = community.CreatedBy,

        };

        var communityMemberEntity = CommunityMemberDomainToEntityMapper.ToEntity(communityMemberDomain);

        await communityMemberRepository.AddAsync(communityMemberEntity, cancellationToken);

        logger.LogInformation(
            "Owner CommunityMember entity staged for insert. CommunityId={CommunityId}, MemberId={MemberId}, MemberType={MemberType}",
            communityMemberEntity.CommunityId, communityMemberEntity.Id, communityMemberEntity.MemberType);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Community creation request successful. CommunityId={CommunityId}, ClubName={ClubName}, OwnerMemberId={MemberId}",
            communityEntity.Id, communityEntity.CommunityName, communityMemberEntity.Id);
    }
}