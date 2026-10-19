using Api.Mapper.ToDomain;
using Api.Requests;
using Application.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Event;

[ApiController]
[Route("api/event")]
public class EventController : ControllerBase
{
    private readonly ICreateEventUseCase useCase;
    public EventController(ICreateEventUseCase useCase)
    {
        this.useCase = useCase;
    }

    [Authorize]
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> HandleAsync(EventRequest request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var accountId))
        {
            throw new UnauthorizedAccessException();
        }

        await useCase.HandleAsync(EventRequestToDomain.ToDomain(request), accountId, cancellationToken);

        return Ok();
    }
}