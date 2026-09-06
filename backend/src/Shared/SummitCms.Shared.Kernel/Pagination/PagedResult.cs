namespace SummitCms.Shared.Kernel.Pagination;

public sealed record PageRequest(int Page = 1, int PageSize = 25)
{
    public int Skip => (Math.Max(Page, 1) - 1) * NormalizedPageSize;
    public int NormalizedPageSize => Math.Clamp(PageSize, 1, 200);
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static PagedResult<T> Create(IReadOnlyList<T> items, int totalCount, PageRequest request) =>
        new(items, totalCount, Math.Max(request.Page, 1), request.NormalizedPageSize);
}
