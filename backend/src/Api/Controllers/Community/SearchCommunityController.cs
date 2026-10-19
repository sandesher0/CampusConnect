using Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    [ProducesResponseType(typeof(PagedResult<Community>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<Community>>> HandleAsync(
        [FromServices] ISearchCommunityFacade searchCommunityFacade,
        [FromRoute] SearchCommunityRequest request,
        [FromQuery] PageRequest page,
        CancellationToken cancellationToken)
    {
        var communities = await searchCommunityFacade.HandleAsync(request.CommunityName, page, cancellationToken);

        return Ok(communities);
    }
}