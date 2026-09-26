namespace Modules.Communities.Ports;

public interface IEnsureCanCreateEventFacade
{
    Task HandleAsync(Guid communityId, Guid userId, CancellationToken cancellationToken);
}