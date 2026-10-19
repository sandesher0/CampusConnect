using System.Security.Claims;
using Api.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Domain;
using Modules.Auth.Ports;

namespace Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class ChangePasswordController : ControllerBase
{
    private readonly IChangePasswordFacade changePasswordFacade;

    public ChangePasswordController(IChangePasswordFacade changePasswordFacade)
    {
        this.changePasswordFacade = changePasswordFacade;
    }

    [HttpPatch("change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> HandleAsync(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var accountId = User.FindFirst("sub")?.Value;

        if (accountId is null)
        {
            throw new UnauthorizedAccessException();
        }

        var changePassword = new ChangePassword
        {
            OldPassword = request.OldPassword,
            NewPassword = request.NewPassword
        };
        await changePasswordFacade.HandleAsync(Guid.Parse(accountId), changePassword, cancellationToken);
        return Ok("Password Changed Successfully");
    }
}