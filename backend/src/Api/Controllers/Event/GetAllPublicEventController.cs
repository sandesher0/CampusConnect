using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Events.Ports;
using SharedKernel.Constants;
using SharedKernel.Response;

namespace Api.Controllers.Event;

[ApiController]
[Route("api/event")]
public class GetAllPublicEventController : ControllerBase
{
    [Authorize]
    [HttpGet("all")]
    [ProducesResponseType(typeof(PagedResult<EventSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResult<EventSummaryResponse>>> HandleAsync(
        [FromServices] IGetAllPublicEventFacade getAllPublicEventFacade,
        [FromQuery] PageRequest page,
        CancellationToken cancellationToken)
    {
        var response = await getAllPublicEventFacade.HandleAsync(page, cancellationToken);
        return Ok(response);
    }
}