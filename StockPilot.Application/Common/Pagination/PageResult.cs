namespace StockPilot.Application.Common.Pagination;

public class PageResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages { get; }

    public PageResult(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalItems)
    {
        if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));

        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

        if (totalItems < 0) throw new ArgumentOutOfRangeException(nameof(totalItems));

        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
    }
}