namespace Application.Contracts.Persistence.Common;

/// <summary>
/// Represents a paged result with data and pagination information
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}

/// <summary>
/// Repository configuration options
/// </summary>
public class RepositoryOptions
{
    public int DefaultPageSize { get; set; } = 10;
    public int MaxPageSize { get; set; } = 1000;
    public bool EnableSoftDelete { get; set; } = true;
    public bool EnableAuditLogging { get; set; } = false;
    public int DefaultBatchSize { get; set; } = 100;
}
