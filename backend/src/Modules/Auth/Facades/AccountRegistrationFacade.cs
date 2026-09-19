using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;
using Modules.Auth.Domain;
using Modules.Auth.Mapper.ToEntity;
using SharedKernel.Interfaces;
using SharedKernel.Exceptions;

namespace Modules.Auth.Facades;

public class AccountRegistrationFacade : IAccountRegistrationFacade
{
    private readonly IAccountRepository accountRepository;
    private readonly ILogger<AccountRegistrationFacade> logger;
    public AccountRegistrationFacade(IAccountRepository accountRepository, ILogger<AccountRegistrationFacade> logger)
    {
        this.accountRepository = accountRepository;
        this.logger = logger;

    }

    public async Task HandleAsync(Account account, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Account Creation for {Username}", account.Username);

        var accountExists = await accountRepository.GetByUsernameAsync(account.Username, cancellationToken);
        if (accountExists is not null)
        {
            logger.LogInformation("Account already exists for provided {Username}", account.Username);
            throw new UsernameAlreadyTakenException(account.Username);
        }

        var newAccount = AccountDomainToEntity.ToEntity(account);
        await accountRepository.AddAsync(newAccount, cancellationToken);

        logger.LogInformation("Created a new Account for {Username}", account.Username);
    }
}