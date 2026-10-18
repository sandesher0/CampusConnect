using Communities.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedKernel.Constants;
using SharedKernel.Response;

namespace Api.Controllers.Communities;

[ApiController]
[Route("api/community")]
public class GetCommunityById : ControllerBase
{
    [Authorize]
    [HttpGet("{communityId:guid}")]
    [ProducesResponseType(typeof(CommunityDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CommunityDetailResponse>> HandleAsync(
        Guid communityId,
        [FromServices] IGetCommunityFacade getCommunityFacade,
        [FromQuery] PageRequest page,
        CancellationToken cancellationToken)
    {
        var response = await getCommunityFacade.HandleAsync(
            communityId,
            page,
            cancellationToken);

        return Ok(response);
    }
}
