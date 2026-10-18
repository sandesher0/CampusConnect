namespace SharedKernel.Constants;

public sealed record PageRequest
{
    public const int MaxPageSize = 50;
    public const int DefaultPageSize = 20;

    private readonly int page = 1;
    private readonly int pageSize = DefaultPageSize;

    public int Page { get => page; init => page = Math.Max(1, value); }
    public int PageSize { get => pageSize; init => pageSize = Math.Clamp(value, 1, MaxPageSize); }
    public int Skip => (page - 1) * PageSize;
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int totalCount, int page, int pageSize)
{
    public int TotalPages => (int)Math.Ceiling(totalCount / (double)pageSize);
    public PagedResult<TOut> Map<TOut>(Func<T, TOut> map) =>
        new(Items.Select(map).ToList(), totalCount, page, pageSize);
}