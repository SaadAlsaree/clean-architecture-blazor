using System.Linq.Expressions;

namespace Application.Contracts.Persistence;

/// <summary>
/// Interface for write operations on entities
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface IWriteRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Creates a single entity
    /// </summary>
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates multiple entities in batches
    /// </summary>
    Task<bool> CreateRangeAsync(
        IEnumerable<TEntity> entities,
        int batchSize = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a single entity
    /// </summary>
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates multiple entities in batches
    /// </summary>
    Task<bool> UpdateRangeAsync(
        IEnumerable<TEntity> entities,
        int batchSize = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates entities based on filter with specific properties
    /// </summary>
    Task<int> UpdateWhereAsync(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TEntity>> updateExpression,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a single entity
    /// </summary>
    Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes entity by ID
    /// </summary>
    Task<bool> DeleteByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes multiple entities in batches
    /// </summary>
    Task<bool> DeleteRangeAsync(
        IEnumerable<TEntity> entities,
        int batchSize = 100,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes entities based on filter
    /// </summary>
    Task<int> DeleteWhereAsync(
        Expression<Func<TEntity, bool>> filter,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all pending changes
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
