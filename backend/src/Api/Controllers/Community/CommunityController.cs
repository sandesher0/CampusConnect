using Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Auth.Ports;
using Modules.Communities.Domain;
using Modules.Communities.Ports;
using SharedKernel.Exceptions;

namespace Api.Controllers;

[ApiController]
[Route("api/community")]
public class CreateCommunityController : ControllerBase
{
    private readonly ICreateCommunityFacade createCommunityFacade;
    private readonly IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade;

    public CreateCommunityController(
        ICreateCommunityFacade createCommunityFacade,
        IGetUserIdByAccountIdFacade getUserIdByAccountIdFacade)
    {
        this.createCommunityFacade = createCommunityFacade;
        this.getUserIdByAccountIdFacade = getUserIdByAccountIdFacade;
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> HandleAsync(CreateCommunityRequest request, CancellationToken cancellationToken)
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

        var community = new Community
        {
            CommunityName = request.CommunityName,
            CommunityType = request.CommunityType,
            CreatedBy = userId.Value
        };
        await createCommunityFacade.HandleAsync(community, cancellationToken);
        return Ok();
    }

}