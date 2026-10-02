using Microsoft.Extensions.Logging;
using Modules.Communities.Domain;
using Modules.Communities.Ports;

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

    public async Task<List<Community>> HandleAsync(
        string searchKeyWord,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Searching communities with keyword: {SearchKeyword}",
            searchKeyWord);

        var communities = await communityRepository
            .SearchCommunityAsync(searchKeyWord, cancellationToken);

        logger.LogInformation(
            "Found {CommunityCount} communities for keyword: {SearchKeyword}",
            communities.Count,
            searchKeyWord);

        return communities;
    }
}
