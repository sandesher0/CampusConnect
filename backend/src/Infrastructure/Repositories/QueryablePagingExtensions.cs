using Microsoft.EntityFrameworkCore;
using SharedKernel.Constants;

public static class QueryablePagingExtensions
{
    public static async Task<PagedResult<T>> ToPagedAsync<T>(
        this IQueryable<T> query, PageRequest request, CancellationToken cancellationToken
    )
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip(request.Skip).Take(request.PageSize).ToListAsync(cancellationToken);
        return new PagedResult<T>(items, total, request.Page, request.PageSize);

    }
}