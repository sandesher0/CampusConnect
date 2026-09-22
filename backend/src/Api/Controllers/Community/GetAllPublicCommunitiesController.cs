using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modules.Communities.Domain;
using Modules.Communities.Ports;

namespace Api.Controllers.Communities;

[ApiController]
[Route("api/communities")]
public class GetAllPublicCommunitiesController : ControllerBase
{
    private readonly IGetAllPublicCommunitiesFacade getAllPublicCommunitiesFacade;

    public GetAllPublicCommunitiesController(IGetAllPublicCommunitiesFacade getAllPublicCommunitiesFacade)
    {
        this.getAllPublicCommunitiesFacade = getAllPublicCommunitiesFacade;
    }

    [Authorize]
    [HttpGet("all")]
    public async Task<List<Community>?> HandleAsync(CancellationToken cancellationToken)
    {
        return await getAllPublicCommunitiesFacade.HandleAsync(cancellationToken);
    }

}