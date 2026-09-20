using Application.Domain;

namespace Application.Ports;

public interface ICreateEventUseCase
{
    Task HandleAsync(Event domain, Guid acountId, CancellationToken cancellationToken);
}