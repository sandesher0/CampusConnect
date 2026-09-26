namespace Modules.Events.Ports;

public interface IAccountLookup
{
    Task<Guid?> GetUserIdByAccountIdAsync(
        Guid accountId,
        CancellationToken cancellationToken);
}