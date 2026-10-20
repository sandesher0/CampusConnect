using SharedKernel.Response;

namespace Application.Ports;

public interface IGetUserLimitsUseCase
{
    Task<UserLimitsResponse> HandleAsync(Guid accountId, CancellationToken cancellationToken);
}
