using Microsoft.AspNetCore.Authorization;
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
    public async Task<ActionResult<PagedResult<EventSummaryResponse>>> HandleAsync(
        [FromServices] IGetAllPublicEventFacade getAllPublicEventFacade,
        [FromQuery] PageRequest page,
        CancellationToken cancellationToken)
    {
        var response = await getAllPublicEventFacade.HandleAsync(page, cancellationToken);
        return Ok(response);
    }
}