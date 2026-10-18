using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Constants;

namespace Api.Controllers.Communities;

[ApiController]
[Route("api/communities")]
public class GetAllPublicCommunitiesController : ControllerBase
{
    private readonly IGetAllPublicCommunitiesFacade getAllPublicCommunitiesFacade;

    public GetAllPublicCommunitiesController(
        IGetAllPublicCommunitiesFacade getAllPublicCommunitiesFacade)
    {
        this.getAllPublicCommunitiesFacade = getAllPublicCommunitiesFacade;
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<ActionResult<PagedResult<Community>>> HandleAsync(
        [FromQuery] PageRequest page,
        CancellationToken cancellationToken)
    {
        var communities = await getAllPublicCommunitiesFacade
            .HandleAsync(page, cancellationToken);

        return Ok(communities);
    }
}
