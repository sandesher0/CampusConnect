using Application.Domain;

namespace Application.Ports;

public interface IRegisterUserUseCase
{
    Task ExecuteAsync(
        RegisterAccount request,
        CancellationToken cancellationToken);
}