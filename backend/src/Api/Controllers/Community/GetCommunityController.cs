using Communities.Ports;
using Microsoft.AspNetCore.Authorization;
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
