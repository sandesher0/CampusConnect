using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Exceptions;

namespace Api.Controllers.Communities;

[ApiController]
[Route("api/community")]
public class GetMyCommunityController : ControllerBase
{
    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<Community>> HandleAsync([FromServices] IGetMyCommunitiesFacade getMyCommunitiesFacade, [FromServices] IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirst("sub")?.Value, out var accountId))
        {
            throw new UnauthorizedAccessException();
        }
        var userId = await getUserIdByAccountIdFacade.HandleAsync(accountId, cancellationToken);

        if (userId is null)
        {
            throw new AccountNotFoundException(accountId);
        }

        var myCommunities = await getMyCommunitiesFacade.HandleAsync(userId.Value, cancellationToken);
        return Ok(myCommunities);
    }
}