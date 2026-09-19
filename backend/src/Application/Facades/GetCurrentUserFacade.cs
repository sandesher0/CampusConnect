using Application.Domain;
using Application.Ports;
using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;
using SharedKernel.Exceptions;

namespace Application.Facades;

public class GetCurrentUserFacade : IGetCurrentUserFacade
{
    private readonly IAccountRepository accountRepository;
    private readonly ILogger<GetCurrentUserFacade> logger;

    public GetCurrentUserFacade(IAccountRepository accountRepository, ILogger<GetCurrentUserFacade> logger)
    {
        this.accountRepository = accountRepository;
        this.logger = logger;
    }

    public async Task<AccountInfo?> HandleAsync(Guid accountId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching the details for User With Account Id : {AccountId}", accountId);

        var accountInfo = await accountRepository.GetByIdAsync(accountId, cancellationToken);
        if (accountInfo is null)
        {
            logger.LogInformation("No Account found for account id {AccountId}", accountId);
            throw new AccountNotFoundException(accountId);
        }

        return new AccountInfo
        {
            Id = accountInfo.Id,
            UserId = accountInfo.UserId,
            Username = accountInfo.Username,
            AccountVerifiedAt = accountInfo.AccountVerifiedAt,
            User = new UserInfo
            {
                Id = accountInfo.User.Id,
                Email = accountInfo.User.Email,
                FirstName = accountInfo.User.FirstName,
                LastName = accountInfo.User.LastName,
                PhoneNumber = accountInfo.User.PhoneNumber,
                ProfileImageUrl = accountInfo.User.ProfileImageUrl
            }
        };
    }
}