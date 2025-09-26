using System.Linq.Expressions;

namespace Application.Contracts.Persistence;

/// <summary>
/// Interface for bulk operations on entities
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface IBulkRepository<TEntity> where TEntity : class
{
    #region Bulk Operations

    /// <summary>
    /// Performs high-performance bulk insert with comprehensive options
    /// Supports simple insert, conflict resolution, and progress reporting
    /// </summary>
    Task<BulkResult> BulkInsertAsync<TKey>(
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, TKey>>? keySelector = null,
        BulkInsertOptions<TEntity>? options = null,
        IProgress<BulkProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs bulk upsert (insert or update) operation
    /// </summary>
    Task<BulkResult> BulkUpsertAsync<TKey>(
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, TKey>>? keySelector = null,
        Expression<Func<TEntity, TEntity, TEntity>>? updateExpression = null,
        BulkInsertOptions<TEntity>? options = null,
        IProgress<BulkProgress>? progress = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Universal bulk import from various data sources (CSV, Excel, SQLite, etc.)
    /// </summary>
    Task<BulkResult> BulkImportFromSourceAsync<TKey, TSource>(
        IEnumerable<TSource> sourceData,
        Expression<Func<TSource, TEntity>> mapper,
        Expression<Func<TEntity, TKey>>? keySelector = null,
        BulkInsertOptions<TEntity>? options = null,
        IProgress<BulkProgress>? progress = null,
        CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// Options for bulk insert operations
/// </summary>
public class BulkInsertOptions<TEntity>
{
    /// <summary>
    /// Number of entities to process in each batch
    /// </summary>
    public int BatchSize { get; set; } = 1000;

    /// <summary>
    /// Whether to use database transactions for bulk operations
    /// </summary>
    public bool UseTransaction { get; set; } = true;

    /// <summary>
    /// Timeout for the bulk operation in seconds
    /// </summary>
    public int TimeoutInSeconds { get; set; } = 300;

    /// <summary>
    /// Strategy for handling conflicts/duplicates
    /// </summary>
    public ConflictResolutionStrategy ConflictStrategy { get; set; } = ConflictResolutionStrategy.Skip;

    /// <summary>
    /// Whether to validate entities before insertion
    /// </summary>
    public bool ValidateEntities { get; set; } = true;

    /// <summary>
    /// Custom validation expression
    /// </summary>
    public Expression<Func<TEntity, bool>>? ValidationExpression { get; set; }

    /// <summary>
    /// Custom error handling strategy
    /// </summary>
    public BulkErrorHandling ErrorHandling { get; set; } = BulkErrorHandling.ContinueOnError;

    /// <summary>
    /// Whether to return detailed results
    /// </summary>
    public bool ReturnDetailedResults { get; set; } = false;
}

/// <summary>
/// Result of bulk operation
/// </summary>
public class BulkResult
{
    public int TotalRecords { get; set; }
    public int SuccessfulInserts { get; set; }
    public int SkippedRecords { get; set; }
    public int UpdatedRecords { get; set; }
    public int ErroredRecords { get; set; }
    public List<BulkError> Errors { get; set; } = new List<BulkError>();
    public TimeSpan Duration { get; set; }
    public bool IsSuccess => ErroredRecords == 0;
}

/// <summary>
/// Represents an error in bulk operation
/// </summary>
public class BulkError
{
    public int RowIndex { get; set; }
    public string? ErrorMessage { get; set; }
    public Exception? Exception { get; set; }
    public object? FailedEntity { get; set; }
}

/// <summary>
/// Progress information for bulk operations
/// </summary>
public class BulkProgress
{
    public int TotalRecords { get; set; }
    public int ProcessedRecords { get; set; }
    public int SuccessfulRecords { get; set; }
    public int ErroredRecords { get; set; }
    public double ProgressPercentage => TotalRecords > 0 ? (double)ProcessedRecords / TotalRecords * 100 : 0;
    public TimeSpan ElapsedTime { get; set; }
    public string? CurrentOperation { get; set; }
}

/// <summary>
/// Strategy for handling conflicts during bulk operations
/// </summary>
public enum ConflictResolutionStrategy
{
    /// <summary>
    /// Skip duplicate records
    /// </summary>
    Skip,
    /// <summary>
    /// Update existing records with new data
    /// </summary>
    Update,
    /// <summary>
    /// Replace existing records completely
    /// </summary>
    Replace,
    /// <summary>
    /// Throw error on conflict
    /// </summary>
    ThrowError
}

/// <summary>
/// Error handling strategy for bulk operations
/// </summary>
public enum BulkErrorHandling
{
    /// <summary>
    /// Throw exception on first error
    /// </summary>
    ThrowOnError,
    /// <summary>
    /// Continue processing and collect errors
    /// </summary>
    ContinueOnError,
    /// <summary>
    /// Skip erroneous records silently
    /// </summary>
    SkipErrors
}
