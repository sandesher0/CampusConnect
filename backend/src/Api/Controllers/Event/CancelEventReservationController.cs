using Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Modules.Events.Ports;
using SharedKernel.Exceptions;

namespace Api.Controllers.Event;

[ApiController]
[Route("api/event")]
public class CancelEventReservationController : ControllerBase
{
    [Authorize]
    [HttpDelete("reservation/{eventId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync(
        [FromServices] IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade,
        [FromServices] ICancelEventReservationFacade cancelEventReservationFacade,
        [FromRoute] CancelEventReservationRequest request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var accountId))
        {
            throw new UnauthorizedAccessException();
        }

        var userId = await getUserIdByAccountIdFacade.HandleAsync(accountId, cancellationToken)
            ?? throw new AccountNotFoundException(accountId);

        await cancelEventReservationFacade.HandleAsync(request.EventId, userId, cancellationToken);

        return NoContent();
    }
}
