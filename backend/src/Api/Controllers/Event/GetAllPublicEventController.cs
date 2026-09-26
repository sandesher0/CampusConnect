using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Events.Domain;
using Modules.Events.Ports;
using SharedKernel.Response;


namespace Api.Controllers.Event;

[ApiController]
[Route("api/event")]
public class GetAllPublicEventController : ControllerBase
{
    [Authorize]
    [HttpGet("all")]
    public async Task<ActionResult<List<EventResponse>>> HandleAsync(IGetAllPublicEventFacade getAllPublicEventFacade, CancellationToken cancellationToken)
    {
        var response = await getAllPublicEventFacade.HandleAsync(cancellationToken);
        return Ok(response);
    }
}