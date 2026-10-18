using System.IdentityModel.Tokens.Jwt;
using Application.Domain;
using Application.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Users;

[ApiController]
[Route("api/user")]
public class MeController : ControllerBase
{
    private readonly IGetCurrentUserFacade getCurrentUserFacade;

    public MeController(IGetCurrentUserFacade getCurrentUserFacade)
    {
        this.getCurrentUserFacade = getCurrentUserFacade;
    }
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(AccountInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> HandleAsync(CancellationToken cancellationToken)
    {
        var accountId = User.FindFirst("sub")?.Value;

        if (accountId is null)
        {
            throw new UnauthorizedAccessException();
        }
        var accountInfo = await getCurrentUserFacade.HandleAsync(Guid.Parse(accountId), cancellationToken);

        return Ok(accountInfo);
    }

}