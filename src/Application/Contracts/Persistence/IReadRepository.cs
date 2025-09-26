using Application.Contracts.Persistence.Common;

namespace Application.Contracts.Persistence;

/// <summary>
/// Interface for read operations on entities
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface IReadRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets a queryable interface for the entity
    /// </summary>
    IQueryable<TEntity> GetQueryable();

    /// <summary>
    /// Builds a query with optional filtering and including (current)
    /// </summary>
    IQueryable<TEntity> Query<TInclude>(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, TInclude>? include = null)
        where TInclude : IQueryable<TEntity>;

    /// <summary>
    /// Builds a query with optional filtering, including, and projection (new)
    /// </summary>
    IQueryable<TResult> Query<TResult>(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, TResult>>? selector = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);

    /// <summary>
    /// Gets entities with comprehensive filtering, ordering, and pagination
    /// </summary>
    Task<IEnumerable<TEntity>> GetAsync<TInclude>(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, TInclude>? include = null,

        int pageSize = -1,
        int pageNumber = 1,
        CancellationToken cancellationToken = default)
        where TInclude : IQueryable<TEntity>;

    /// <summary>
    /// Gets entities with projection and pagination
    /// </summary>
    Task<PagedResult<TResult>> GetPagedAsync<TResult, TInclude>(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, TResult>>? selector = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        Func<IQueryable<TEntity>, TInclude>? include = null,
        int pageSize = 10,
        int pageNumber = 1,
        CancellationToken cancellationToken = default)
        where TInclude : IQueryable<TEntity>;

    /// <summary>
    /// Finds a single entity based on filter
    /// </summary>
    Task<TEntity> FindAsync<TInclude>(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, TInclude>? include = null,
        CancellationToken cancellationToken = default)
        where TInclude : IQueryable<TEntity>;

    /// <summary>
    /// Finds a single entity by ID
    /// </summary>
    Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity matches the filter
    /// </summary>
    Task<bool> ExistsAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets count of entities matching the filter
    /// </summary>
    Task<int> CountAsync<TInclude>(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, TInclude>? include = null,
        CancellationToken cancellationToken = default)
        where TInclude : IQueryable<TEntity>;

    /// <summary>
    /// Executes raw SQL query and returns entities
    /// </summary>
    Task<IEnumerable<TEntity>> ExecuteRawSqlAsync(
        string sql,
        CancellationToken cancellationToken = default,
        params object[] parameters);

    /// <summary>
    /// Finds a single entity with optional projection or include
    /// </summary>
    Task<TResult> FindAsync<TResult>(
        Expression<Func<TEntity, bool>>? filter = null,
        Expression<Func<TEntity, TResult>>? selector = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
        CancellationToken cancellationToken = default);
}
