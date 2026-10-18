using Microsoft.Extensions.Logging;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Constants;

public class GetAllPublicCommunitiesFacade : IGetAllPublicCommunitiesFacade
{
    private readonly ICommunityRepository communityRepository;
    private readonly ILogger<GetAllPublicCommunitiesFacade> logger;

    public GetAllPublicCommunitiesFacade(ICommunityRepository communityRepository, ILogger<GetAllPublicCommunitiesFacade> logger)
    {
        this.communityRepository = communityRepository;
        this.logger = logger;
    }

    public async Task<PagedResult<Community>> HandleAsync(PageRequest page, CancellationToken cancellationToken)
    {
        var communities = await communityRepository.GetAllPublicCommunitiesAsync(page, cancellationToken);
        return communities;
    }
}