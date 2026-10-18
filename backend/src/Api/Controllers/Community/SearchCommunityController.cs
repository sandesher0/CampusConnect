using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Constants;

namespace Api.Controllers.Communities;

[ApiController]
[Route("api/community")]
public class SearchCommunityController : ControllerBase
{
    [HttpGet("search/{communityName}")]
    [Authorize]
    public async Task<ActionResult<PagedResult<Community>>> HandleAsync(
        [FromServices] ISearchCommunityFacade searchCommunityFacade,
        string communityName,
        [FromQuery] PageRequest page,
        CancellationToken cancellationToken)
    {
        var communities = await searchCommunityFacade.HandleAsync(communityName, page, cancellationToken);

        return Ok(communities);
    }
}