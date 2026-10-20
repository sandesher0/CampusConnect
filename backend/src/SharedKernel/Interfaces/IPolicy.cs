using SharedKernel.Models;

namespace SharedKernel.Interfaces;

public interface IPolicy<in TContext>
{
    Task<PolicyResult> EvaluateAsync(TContext context, CancellationToken cancellationToken = default);
}
