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
    private const string CreationLockScope = "Communities.Create";

    private readonly ICommunityRepository repository;
    private readonly ICommunityMemberRepository communityMemberRepository;
    private readonly ICommunityCreationPolicy communityCreationPolicy;
    private readonly ILimitProvider limitProvider;
    private readonly ILogger<CreateCommunityFacade> logger;
    private readonly IUnitOfWork unitOfWork;

    public CreateCommunityFacade(
        ICommunityRepository repository,
        ICommunityMemberRepository communityMemberRepository,
        ICommunityCreationPolicy communityCreationPolicy,
        ILimitProvider limitProvider,
        ILogger<CreateCommunityFacade> logger,
        IUnitOfWork unitOfWork)
    {
        this.repository = repository;
        this.communityMemberRepository = communityMemberRepository;
        this.communityCreationPolicy = communityCreationPolicy;
        this.limitProvider = limitProvider;
        this.logger = logger;
        this.unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(Community community, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Community creation started. CreatedBy={UserId}, ClubName={ClubName}, ClubType={ClubType}",
            community.CreatedBy, community.CommunityName, community.CommunityType);

        await unitOfWork.ExecuteInTransactionAsync(async ct =>
        {
            // 1. Serialize concurrent community creation requests for this specific user
            await unitOfWork.AcquireAdvisoryLockAsync(CreationLockScope, community.CreatedBy, ct);

            // 2. Evaluate the count AFTER acquiring the lock so it reads committed rows from any prior request
            var policyResult = await communityCreationPolicy.EvaluateAsync(community.CreatedBy, ct);
            if (!policyResult.Succeeded)
            {
                throw new PlanLimitExceededException(policyResult.LimitKey!, policyResult.Limit!.Value);
            }

            var memberLimit = await limitProvider.GetLimitAsync(
                community.CreatedBy,
                LimitKeys.CommunityMaxMember,
                ct);
            if (memberLimit.Value is null)
            {
                throw new InvalidOperationException(
                    $"Configuration error: Limit key '{LimitKeys.CommunityMaxMember}' must have a numeric value.");
            }

            community.MaximumMembers = memberLimit.Value.Value;

            // 3. Stage Community and Owner Member entities
            var communityEntity = CommunityDomainToEntityMapper.ToEntity(community);
            await repository.AddAsync(communityEntity, ct);

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
            await communityMemberRepository.AddAsync(communityMemberEntity, ct);

            logger.LogInformation(
                "Owner CommunityMember entity staged for insert. CommunityId={CommunityId}, MemberId={MemberId}, MemberType={MemberType}",
                communityMemberEntity.CommunityId, communityMemberEntity.Id, communityMemberEntity.MemberType);

            // 4. Flush inserts to DB before committing the transaction and releasing the advisory lock
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation(
                "Community creation staged for commit. CommunityId={CommunityId}, ClubName={ClubName}, OwnerMemberId={MemberId}",
                communityEntity.Id, communityEntity.CommunityName, communityMemberEntity.Id);
        }, cancellationToken);

        logger.LogInformation(
            "Community creation transaction committed successfully. CreatedBy={UserId}, ClubName={ClubName}",
            community.CreatedBy, community.CommunityName);
    }
}
