using System.Reflection.Metadata;
using Application.Domain;

namespace Application.Ports;

public interface IGetCurrentUserFacade
{
    Task<AccountInfo?> HandleAsync(Guid accountId, CancellationToken cancellationToken);
}