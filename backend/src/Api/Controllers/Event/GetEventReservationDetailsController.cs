using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Response;
namespace Api.Controllers.Event;

[ApiController]
[Route("api/event")]
public class GetEventReservationDetailsController : ControllerBase
{
    [Authorize]
    [HttpGet("reservation/{eventId:guid}")]
    [ProducesResponseType(typeof(List<EventReservationSummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<EventReservationSummaryResponse>>> HandleAsync(
        [FromServices] IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade,
        [FromServices] IGetEventReservationDetailsFacade getEventReservationDetailsFacade,
        Guid eventId,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var accountId))
        {
            throw new UnauthorizedAccessException();
        }

        var userId = await getUserIdByAccountIdFacade.HandleAsync(accountId, cancellationToken)
            ?? throw new AccountNotFoundException(accountId);

        var reservations = await getEventReservationDetailsFacade.HandleAsync(userId, eventId, cancellationToken);

        return Ok(reservations);
    }
}