namespace Modules.Auth.Facades;

using Microsoft.Extensions.Logging;
using Modules.Auth.Domain;
using Modules.Auth.Ports;
using SharedKernel.Exceptions;

public class LoginFacade : IAccountLoginFacade
{
    private readonly IAccountRepository accountRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly ITokenGenerator tokenGenerator;
    private readonly ILogger<LoginFacade> logger;

    public LoginFacade(
        IAccountRepository accountRepository,
        ILogger<LoginFacade> logger,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator)
    {
        this.accountRepository = accountRepository;
        this.logger = logger;
        this.passwordHasher = passwordHasher;
        this.tokenGenerator = tokenGenerator;
    }

    public async Task<string> HandleAsync(
        Login login,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Login attempt for account with Username {Username}",
            login.Username);

        var account = await accountRepository
            .GetByUsernameAsync(login.Username, cancellationToken);

        if (account is null)
        {
            logger.LogWarning(
                "Login failed: account not found for Username {Username}",
                login.Username);

            throw new InvalidCredentialsException();
        }

        var isPasswordMatch = await passwordHasher.ComparePassword(
            login.Password,
            account.PasswordHash);

        if (!isPasswordMatch)
        {
            logger.LogWarning(
                "Login failed: invalid password for Username {Username}",
                login.Username);

            throw new InvalidCredentialsException();
        }

        var accessToken = tokenGenerator.GenerateJwtToken(account);

        logger.LogInformation(
            "Login successful for account with Username {Username}",
            login.Username);

        return accessToken;
    }
}
