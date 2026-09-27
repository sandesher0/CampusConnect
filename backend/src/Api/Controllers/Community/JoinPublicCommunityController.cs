using Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Communities;

[ApiController]
[Route("api/community")]
public class JoinPublicCommunityController : ControllerBase
{
    [HttpPost("{communityId:guid}/join")]
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