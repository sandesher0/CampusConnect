using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;

namespace Modules.Auth.Facades;

public class GetUserIdByAccountIdFacade : IGetUserIdByAccountIdFacade
{
    private readonly IAccountRepository accountRepository;
    private readonly ILogger<GetUserIdByAccountIdFacade> logger;

    public GetUserIdByAccountIdFacade(IAccountRepository accountRepository, ILogger<GetUserIdByAccountIdFacade> logger)
    {
        this.accountRepository = accountRepository;
        this.logger = logger;
    }

    public async Task<Guid?> HandleAsync(Guid accountId, CancellationToken cancellationToken)
    {
        var userId = await accountRepository.GetUserIdByAccountIdAsync(accountId, cancellationToken);
        logger.LogDebug("Resolved AccountId={AccountId} to UserId={UserId}", accountId, userId);
        return userId;
    }
}