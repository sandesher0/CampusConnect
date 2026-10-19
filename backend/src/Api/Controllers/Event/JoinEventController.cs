using Api.Requests;
using Application.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Event;

[ApiController]
[Route("api/event")]
public class JoinEventController : ControllerBase
{

    [HttpPost("join")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> HandleAsync(
        [FromServices] IJoinEventUseCase joinEventUseCase,
        [FromQuery] JoinEventRequest request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var accountId))
        {
            throw new UnauthorizedAccessException();
        }

        await joinEventUseCase.HandleAsync(accountId, request.EventId, request.CommunityId, cancellationToken);

        return Ok();
    }
}