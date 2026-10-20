namespace SharedKernel.Response;

public record LimitDetailResponse(int? Limit, int? CurrentUsage, bool IsUnlimited);

public record UserLimitsResponse(
    string PlanCode,
    string PlanName,
    IReadOnlyDictionary<string, LimitDetailResponse> Limits);
