namespace Modules.Auth.Ports;

public interface IGetUserIdByAccountIdFacade
{
    Task<Guid?> HandleAsync(Guid accountId, CancellationToken cancellationToken);
}