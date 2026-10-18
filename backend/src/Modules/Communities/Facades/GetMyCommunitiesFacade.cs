using Microsoft.Extensions.Logging;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Constants;

namespace Modules.Communities.Facades;

public class GetMyCommunityFacade : IGetMyCommunitiesFacade
{
    private readonly ICommunityRepository repository;
    private readonly ILogger<GetMyCommunityFacade> logger;

    public GetMyCommunityFacade(
        ICommunityRepository repository,
        ILogger<GetMyCommunityFacade> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public async Task<PagedResult<Community>> HandleAsync(
        Guid userId,
        PageRequest page,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Fetching communities for user {UserId}",
            userId);

        var communities = await repository.GetMyCommunitiesAsync(
            userId,
            page,
            cancellationToken);

        logger.LogInformation(
            "Found {CommunityCount} communities for user {UserId}",
            communities.totalCount,
            userId);

        return communities;
    }
}
