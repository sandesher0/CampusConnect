using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Modules.Auth.Services;

namespace Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class LogoutController : ControllerBase
{
    private readonly IAccountRepository accountRepository;

    public LogoutController(IAccountRepository accountRepository)
    {
        this.accountRepository = accountRepository;
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> HandleAsync(CancellationToken cancellationToken)
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var refreshTokenHash = RefreshTokenHasher.Hash(refreshToken);
            await accountRepository.RemoveRefreshTokenAsync(refreshTokenHash, cancellationToken);
        }

        Response.Cookies.Delete("accessToken", AuthCookieOptions.DeleteAccessToken());
        Response.Cookies.Delete("refreshToken", AuthCookieOptions.DeleteRefreshToken());
        return Ok();
    }
}
