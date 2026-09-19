using Application.Ports;
using Application.Domain;
using Modules.Auth.Ports;
using Modules.Users.Ports;
using Microsoft.Extensions.Logging;
using Modules.Users.Domain;
using Modules.Auth.Domain;
using SharedKernel.Interfaces;


public class RegisterUserUseCase : IRegisterUserUseCase
{
    private readonly IAccountRegistrationFacade accountRegistrationFacade;
    private readonly IUserRegistrationFacade userRegistrationFacade;
    private readonly IPasswordHasher passwordHasher;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILogger<RegisterUserUseCase> logger;

    public RegisterUserUseCase(
        IAccountRegistrationFacade accountRegistrationFacade,
        IUserRegistrationFacade userRegistrationFacade,
        IPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ILogger<RegisterUserUseCase> logger)
    {
        this.accountRegistrationFacade = accountRegistrationFacade;
        this.userRegistrationFacade = userRegistrationFacade;
        this.passwordHasher = passwordHasher;
        this.unitOfWork = unitOfWork;
        this.logger = logger;
    }

    public async Task ExecuteAsync(RegisterAccount registerAccount, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = registerAccount.Email,
            FirstName = registerAccount.FirstName,
            LastName = registerAccount.LastName,
        };
        await userRegistrationFacade.HandleAsync(user, cancellationToken);

        string hashedPassword = await passwordHasher.HashPassword(registerAccount.Password);

        var account = new Account
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Username = registerAccount.Username,
            PasswordHash = hashedPassword
        };

        await accountRegistrationFacade.HandleAsync(account, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

    }
}