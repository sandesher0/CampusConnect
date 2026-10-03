using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Events.Ports;
using SharedKernel.Response;

namespace Api.Controllers.Event;


[ApiController]
[Route("api/event")]
public class GetEventController : ControllerBase
{
    [HttpGet("{eventId:guid}")]
    [Authorize]
    public async Task<ActionResult<EventResponse>> HandleAsync([FromServices] IGetEventFacade getEventFacade, Guid eventId, CancellationToken cancellationToken)
    {
        var eventResponse = await getEventFacade.HandleAsync(eventId, cancellationToken);
        return Ok(eventResponse);
    }
}