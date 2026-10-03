namespace Application.Ports;

public interface IJoinEventUseCase
{
    Task HandleAsync(Guid accountId, Guid eventId, Guid communityId, CancellationToken cancellationToken);
}
