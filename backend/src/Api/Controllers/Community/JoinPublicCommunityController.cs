using Application.Ports;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Communities;

[ApiController]
[Route("api/community")]
public class JoinPublicCommunityController : ControllerBase
{
    [HttpPost("{communityId:guid}/join")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> HandleAsync(Guid communityId, [FromServices] IJoinPublicCommunityUseCase joinPublicCommunityUseCase, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var accountId))
        {
            throw new UnauthorizedAccessException();
        }
        await joinPublicCommunityUseCase.HandleAsync(accountId, communityId, cancellationToken);
        return Ok();
    }
}