namespace Application.Contracts.Persistence;

/// <summary>
/// Interface for extension operations on entities
/// </summary>
/// <typeparam name="TId">The ID type</typeparam>
public interface IExtensionRepository<TId> : IDisposable
{
    /// <summary>
    /// Deletes a record by table name and primary ID
    /// </summary>
    Task<bool> DeleteRecordAsync(string tableName, TId primaryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a record (sets IsDeleted = true)
    /// </summary>
    Task<bool> SoftDeleteRecordAsync(string tableName, TId primaryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Restores a soft deleted record
    /// </summary>
    Task<bool> RestoreRecordAsync(string tableName, TId primaryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes the status of a record
    /// </summary>
    Task<bool> ChangeStatusAsync<TStatus>(string tableName, TId primaryId, TStatus newStatus, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the active status of a record
    /// </summary>
    Task<bool> ToggleActiveStatusAsync(string tableName, TId primaryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple records by IDs
    /// </summary>
    Task<int> DeleteRecordsAsync(string tableName, IEnumerable<TId> primaryIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes status of multiple records
    /// </summary>
    Task<int> ChangeStatusBatchAsync<TStatus>(string tableName, IEnumerable<TId> primaryIds, TStatus newStatus, CancellationToken cancellationToken = default);
}
