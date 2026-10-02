using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class LogoutController : ControllerBase
{
    [HttpPost("logout")]
    public async Task<IActionResult> HandleAsync(CancellationToken cancellationToken)
    {
        Response.Cookies.Delete("accessToken", new CookieOptions
        {
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
            Secure = true
        });
        return Ok();
    }
}