namespace SharedKernel.Models;

public readonly record struct PolicyResult(bool Succeeded, string? ErrorMessage = null, string? LimitKey = null, int? Limit = null)
{
    public static PolicyResult Success() => new(true);
    public static PolicyResult Failure(string errorMessage, string limitKey, int? limit) => new(false, errorMessage, limitKey, limit);
}
