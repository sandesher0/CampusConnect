using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Communities.Domain;
using Modules.Communities.Ports;

namespace Api.Controllers.Communities;

[ApiController]
[Route("api/community")]
public class SearchCommunityController : ControllerBase
{

    [HttpGet("search/{communityName}")]
    [Authorize]
    public async Task<ActionResult<List<Community>>> HandleAsync([FromServices] ISearchCommunityFacade searchCommunityFacade, string communityName, CancellationToken cancellationToken)
    {
        var communities = await searchCommunityFacade.HandleAsync(communityName, cancellationToken);

        return Ok(communities);
    }

}