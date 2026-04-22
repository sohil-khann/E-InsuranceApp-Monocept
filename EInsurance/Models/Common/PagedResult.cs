namespace EInsurance.Models.Common;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasNextPage => CurrentPage < TotalPages;
    public bool HasPreviousPage => CurrentPage > 1;
    public int FirstRecordIndex => TotalRecords == 0 ? 0 : ((CurrentPage - 1) * PageSize) + 1;
    public int LastRecordIndex => Math.Min(CurrentPage * PageSize, TotalRecords);

    public PagedResult() { }

    public PagedResult(List<T> items, int totalRecords, int currentPage, int pageSize)
    {
        Items = items;
        TotalRecords = totalRecords;
        CurrentPage = currentPage;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
    }
}

public class PaginationQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = PaginationDefaults.DefaultPageSize;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = true;
    public string? SearchTerm { get; set; }
}

public static class PaginationDefaults
{
    public const int DefaultPageSize = 10;
    public const int MinPageSize = 5;
    public const int MaxPageSize = 100;
    public static readonly int[] PageSizeOptions = { 10, 20, 50, 100 };
}