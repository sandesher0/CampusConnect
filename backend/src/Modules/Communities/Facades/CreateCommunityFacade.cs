using Modules.Communities.Domain;
using Modules.Communities.Ports;
using Modules.Communities.Policies;
using Modules.Communities.Mapper.ToEntities;
using Microsoft.Extensions.Logging;
using SharedKernel.Interfaces;
using SharedKernel.Constants;
using SharedKernel.Exceptions;

namespace Modules.Communities.Facades;

public class CreateCommunityFacade : ICreateCommunityFacade
{
    private readonly ICommunityRepository repository;
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ICommunityCreationPolicy communityCreationPolicy;
    private readonly ILogger<CreateCommunityFacade> logger;
    private readonly IUnitOfWork unitOfWork;

    public CreateCommunityFacade(
        ICommunityRepository repository,
        ICommunityMemberRepository communityMemberRepository,
        ICommunityCreationPolicy communityCreationPolicy,
        ILogger<CreateCommunityFacade> logger,
        IUnitOfWork unitOfWork)
    {
        this.repository = repository;
        this.communityMemberRepository = communityMemberRepository;
        this.communityCreationPolicy = communityCreationPolicy;
        this.logger = logger;
        this.unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(Community community, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Community creation started. CreatedBy={UserId}, ClubName={ClubName}, ClubType={ClubType}",
            community.CreatedBy, community.CommunityName, community.CommunityType);

        var policyResult = await communityCreationPolicy.EvaluateAsync(community.CreatedBy, cancellationToken);
        if (!policyResult.Succeeded)
        {
            throw new PlanLimitExceededException(policyResult.LimitKey!, policyResult.Limit!.Value);
        }

        var communityEntity = CommunityDomainToEntityMapper.ToEntity(community);

        await repository.AddAsync(communityEntity, cancellationToken);

        logger.LogInformation(
            "Community entity staged for insert. CommunityId={CommunityId}, ClubName={ClubName}",
            communityEntity.Id, communityEntity.CommunityName);

        var communityMemberDomain = new CommunityMember
        {
            CommunityId = communityEntity.Id,
            MemberType = MemberType.ClubOfficer,
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