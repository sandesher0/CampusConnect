using System.IdentityModel.Tokens.Jwt;
using Application.Ports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Users;

[ApiController]
[Route("api/user")]
public class MeController : ControllerBase
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IGetCurrentUserFacade getCurrentUserFacade;

    public MeController(IHttpContextAccessor httpContextAccessor, IGetCurrentUserFacade getCurrentUserFacade)
    {
        this.httpContextAccessor = httpContextAccessor;
        this.getCurrentUserFacade = getCurrentUserFacade;
    }
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> HandleAsync(CancellationToken cancellationToken)
    {
        var accountId = User.FindFirst("sub")?.Value; ;

        if (accountId is null)
        {
            throw new InvalidOperationException("Invalid token");
        }
        var accountInfo = await getCurrentUserFacade.HandleAsync(Guid.Parse(accountId), cancellationToken);

        return Ok(accountInfo);
    }

}