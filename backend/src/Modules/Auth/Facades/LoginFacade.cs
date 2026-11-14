namespace Modules.Auth.Facades;

using Microsoft.Extensions.Logging;
using Modules.Auth.Constants;
using Modules.Auth.Domain;
using Modules.Auth.Ports;
using Modules.Auth.Services;
using SharedKernel.Entities;
using SharedKernel.Exceptions;
using SharedKernel.Interfaces;

public class LoginFacade : IAccountLoginFacade
{
    private readonly IAccountRepository accountRepository;
    private readonly IPasswordHasher passwordHasher;
    private readonly ITokenGenerator tokenGenerator;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<LoginFacade> logger;

    public LoginFacade(
        IAccountRepository accountRepository,
        ILogger<LoginFacade> logger,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork)
    {
        this.accountRepository = accountRepository;
        this.logger = logger;
        this.passwordHasher = passwordHasher;
        this.tokenGenerator = tokenGenerator;
        this.unitOfWork = unitOfWork;
    }

    public async Task<LoginTokens> HandleAsync(
        Login login,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Login attempt for account with Username {Username}",
            login.Username);

        var accountEntity = await accountRepository
            .GetByUsernameForLoginAsync(login.Username, cancellationToken);

        if (accountEntity is null)
        {
            logger.LogWarning(
                "Login failed: account not found for Username {Username}",
                login.Username);

            throw new InvalidCredentialsException();
        }

        var isPasswordMatch = await passwordHasher.ComparePassword(
            login.Password,
            accountEntity.PasswordHash);

        if (!isPasswordMatch)
        {
            logger.LogWarning(
                "Login failed: invalid password for Username {Username}",
                login.Username);

            throw new InvalidCredentialsException();
        }

        var nowUtc = DateTimeOffset.UtcNow;
        await accountRepository.PruneExpiredRefreshTokensAsync(accountEntity.Id, nowUtc, cancellationToken);

        var account = new Account
        {
            Id = accountEntity.Id,
            UserId = accountEntity.UserId,
            Username = accountEntity.Username,
            PasswordHash = accountEntity.PasswordHash,
            IsActive = accountEntity.IsActive,
            AccountVerifiedAt = accountEntity.AccountVerifiedAt,
            FailedLoginAttempts = accountEntity.FailedLoginAttempts
        };

        var accessToken = tokenGenerator.GenerateJwtToken(account);
        var refreshToken = tokenGenerator.GenerateRefreshToken();
        var refreshTokenHash = RefreshTokenHasher.Hash(refreshToken);

        await accountRepository.AddRefreshTokenAsync(new RefreshTokenEntity
        {
            Id = Guid.NewGuid(),
            AccountId = accountEntity.Id,
            TokenHash = refreshTokenHash,
            ExpiresAtUtc = nowUtc.Add(AuthTokenLifetimes.RefreshTokenLifetime),
            CreatedAtUtc = nowUtc
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Login successful for account with Username {Username}",
            login.Username);

        return new LoginTokens
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}
