using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Modules.Events.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Response;

namespace Api.Controllers.Event;

[ApiController]
[Route("api/event")]
public class GetMyEventReservationsController : ControllerBase
{
    [Authorize]
    [HttpGet("reservations/mine")]
    [ProducesResponseType(typeof(List<MyEventReservationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<MyEventReservationResponse>>> HandleAsync(
        [FromServices] IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade,
        [FromServices] IGetMyEventReservationsFacade getMyEventReservationsFacade,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var accountId))
        {
            throw new UnauthorizedAccessException();
        }

        var userId = await getUserIdByAccountIdFacade.HandleAsync(accountId, cancellationToken)
            ?? throw new AccountNotFoundException(accountId);

        var reservations = await getMyEventReservationsFacade.HandleAsync(userId, cancellationToken);

        return Ok(reservations);
    }
}
