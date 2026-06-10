namespace BasePlatform.Shared;

public sealed record PaginatedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount)
{
    // public int TotalPages =>
    //     PageSize <= 0
    //         ? 0
    //         : (int)Math.Ceiling((double)TotalCount / PageSize);

    public int TotalPages
    {
        get
        {
            if (PageSize <= 0)
            {
                return 0;
            }
            else
            {
                return (int)Math.Ceiling((double)TotalCount / PageSize);
            }
        }
    }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static PaginatedResult<T> Empty(int pageNumber = 1, int pageSize = 10) =>
        new(Array.Empty<T>(), pageNumber, pageSize, 0);
}