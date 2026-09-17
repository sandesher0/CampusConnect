using Microsoft.Extensions.Logging;
using Modules.Auth.Ports;
using Modules.Auth.Domain;
using Modules.Auth.Mapper.ToEntity;
using SharedKernel.Interfaces;
using SharedKernel.Exceptions;

namespace Modules.Auth.Facades;

public class UserRegistrationFacade : IUserRegistrationFacade
{
    private readonly IUserRegistrationRepository userRegistrationRepository;
    private readonly ILogger<UserRegistrationFacade> logger;

    private readonly IUnitOfWork unitOfWork;
    public UserRegistrationFacade(IUserRegistrationRepository userRegistrationRepository, ILogger<UserRegistrationFacade> logger, IUnitOfWork unitOfWork)
    {
        this.userRegistrationRepository = userRegistrationRepository;
        this.logger = logger;
        this.unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(Account account, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting Account Creation for {Email}:  ", account.Email);

        var accountExists = await userRegistrationRepository.GetByEmailAsync(account.Email, cancellationToken);
        if (accountExists is not null)
        {
            logger.LogInformation("Account already exists for provided {Email}:", account.Email);
            throw new AccountAlreadyExistsException(account.Email);
        }

        var newAccount = AccountDomainToEntity.ToEntity(account);
        await userRegistrationRepository.AddAsync(newAccount, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Created a new Account for {Email}", account.Email);
    }
}