
using API.Requests;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Domain;
using Modules.Auth.Ports;

namespace API.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class LoginController : ControllerBase
{
    private readonly IAccountLoginFacade loginFacade;

    public LoginController(IAccountLoginFacade loginFacade)
    {
        this.loginFacade = loginFacade;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest, CancellationToken cancellationToken)
    {
        var login = new Login
        {
            Username = loginRequest.Username,
            Password = loginRequest.Password,
        };
        var accessToken = await loginFacade.HandleAsync(login, cancellationToken);

        Response.Cookies.Append(
            "accessToken",
            accessToken,
            new CookieOptions
            {
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
                Secure = true

            }
        );
        return Ok();
    }
}