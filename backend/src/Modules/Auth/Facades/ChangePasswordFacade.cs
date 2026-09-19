using Microsoft.Extensions.Logging;
using Modules.Auth.Domain;
using Modules.Auth.Ports;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;

namespace Modules.Auth.Facades;

public class ChangePasswordFacade : IChangePasswordFacade
{
    private readonly IAccountRepository accountRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<ChangePasswordFacade> logger;

    public ChangePasswordFacade(IAccountRepository accountRepository,
     ILogger<ChangePasswordFacade> logger,
     IUnitOfWork unitOfWork,
     IPasswordHasher passwordHasher)
    {
        this.accountRepository = accountRepository;
        this.logger = logger;
        this.passwordHasher = passwordHasher;
        this.unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(Guid accountId, ChangePassword resetPassword, CancellationToken cancellationToken)
    {
        logger.LogInformation(
        "Password reset requested for account {AccountId}",
        accountId);

        var accountEntity = await accountRepository.GetByIdAsync(accountId, cancellationToken);

        if (accountEntity is null)
        {
            logger.LogWarning(
            "Password reset failed because account {AccountId} was not found",
            accountId);
            throw new AccountNotFoundException(accountId);
        }

        bool isPasswordMatched = await passwordHasher.ComparePassword(
            resetPassword.OldPassword,
            accountEntity.PasswordHash);

        if (!isPasswordMatched)
        {
            logger.LogWarning(
            "Password reset failed due to invalid current password for account {AccountId}",
            accountId);
            throw new InvalidCredentialsException();
        }

        var newHashedPassword = await passwordHasher.HashPassword(resetPassword.NewPassword);

        accountEntity.PasswordHash = newHashedPassword;
        await accountRepository.UpdateAsync(accountEntity);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation(
        "Password reset completed successfully for account {AccountId}",
        accountId);
    }
}