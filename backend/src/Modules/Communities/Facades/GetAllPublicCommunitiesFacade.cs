using Modules.Communities.Ports;
using Modules.Communities.Domain;
using SharedKernel.Interfaces;
using Microsoft.Extensions.Logging;

public class GetAllPublicCommunitiesFacade : IGetAllPublicCommunitiesFacade
{
    private readonly ICommunityRepository communityRepository;
    private readonly ILogger<GetAllPublicCommunitiesFacade> logger;

    public GetAllPublicCommunitiesFacade(ICommunityRepository communityRepository, ILogger<GetAllPublicCommunitiesFacade> logger)
    {
        this.communityRepository = communityRepository;
        this.logger = logger;
    }

    public async Task<List<Community>?> HandleAsync(CancellationToken cancellationToken)
    {
        var communities = await communityRepository.GetAllPublicCommunitiesAsync(cancellationToken);
        return communities;
    }
}