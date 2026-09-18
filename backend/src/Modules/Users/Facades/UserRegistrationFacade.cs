using Microsoft.Extensions.Logging;
using Modules.Users.Ports;
using Modules.Users.Domain;
using SharedKernel.Interfaces;
using SharedKernel.Exceptions;
using Modules.Users.Mapper.ToEntity;

namespace Modules.Users.Facades;

public class UserRegistrationFacade : IUserRegistrationFacade
{
    private readonly IUserRegistrationRepository userRegistrationRepository;
    private readonly ILogger<UserRegistrationFacade> logger;
    private readonly IUnitOfWork unitOfWork;

    public UserRegistrationFacade(
        IUserRegistrationRepository userRegistrationRepository,
        ILogger<UserRegistrationFacade> logger,
        IUnitOfWork unitOfWork)
    {
        this.userRegistrationRepository = userRegistrationRepository;
        this.logger = logger;
        this.unitOfWork = unitOfWork;
    }

    public async Task HandleAsync(User user, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting User Registration for {Email}", user.Email);

        var existingUser = await userRegistrationRepository.FindByEmailAsync(user.Email!, cancellationToken);
        if (existingUser is not null)
        {
            logger.LogInformation("Account already exists for provided Email: {Email}", user.Email);
            throw new AccountAlreadyExistsException(user.Email!);
        }

        var userEntity = UserDomainToEntity.ToEntity(user);

        await userRegistrationRepository.AddAsync(userEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Successfully registered User {Email}", user.Email);
    }
}
