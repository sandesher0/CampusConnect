using Microsoft.Extensions.Logging;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Constants;

public class SearchCommunityFacade : ISearchCommunityFacade
{
    private readonly ICommunityRepository communityRepository;
    private readonly ILogger<SearchCommunityFacade> logger;

    public SearchCommunityFacade(
        ICommunityRepository communityRepository,
        ILogger<SearchCommunityFacade> logger)
    {
        this.communityRepository = communityRepository;
        this.logger = logger;
    }

    public async Task<PagedResult<Community>> HandleAsync(
        string searchKeyWord,
        PageRequest page,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Searching communities with keyword: {SearchKeyword}",
            searchKeyWord);

        var communities = await communityRepository
            .SearchCommunityAsync(searchKeyWord, page, cancellationToken);

        logger.LogInformation(
            "Found {CommunityCount} communities for keyword: {SearchKeyword}",
            communities.totalCount,
            searchKeyWord);

        return communities;
    }
}
