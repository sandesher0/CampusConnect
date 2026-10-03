using Application.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Event;

[ApiController]
[Route("api/event")]
public class JoinEventController : ControllerBase
{

    [HttpPost("join")]
    [Authorize]
    public async Task<IActionResult> HandleAsync([FromServices] IJoinEventUseCase joinEventUseCase, Guid communityId, Guid eventId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var accountId))
        {
            throw new UnauthorizedAccessException();
        }

        await joinEventUseCase.HandleAsync(accountId, eventId, communityId, cancellationToken);

        return Ok();
    }
}