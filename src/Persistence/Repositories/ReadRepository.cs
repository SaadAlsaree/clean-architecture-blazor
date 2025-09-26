
namespace Persistence.Repositories;

internal class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public ReadRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<int> CountAsync<TInclude>(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, TInclude>? include = null, CancellationToken cancellationToken = default) where TInclude : IQueryable<TEntity>
    {
        IQueryable<TEntity> query = _dbSet;

        if (include != null)
            query = include(query);

        if (filter != null)
            query = query.Where(filter);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> ExecuteRawSqlAsync(string sql, CancellationToken cancellationToken = default, params object[] parameters)
    {
        return await _dbSet.FromSqlRaw(sql, parameters).ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
            query = query.Where(filter);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<TEntity> FindAsync<TInclude>(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, TInclude>? include = null, CancellationToken cancellationToken = default) where TInclude : IQueryable<TEntity>
    {
        IQueryable<TEntity> query = _dbSet;

        if (include != null)
            query = include(query);

        if (filter != null)
            query = query.Where(filter);

        return await query.FirstOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException("No entity found matching the criteria.");
    }

    public async Task<TResult> FindAsync<TResult>(Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, TResult>>? selector = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet;

        if (include != null)
            query = include(query);

        if (filter != null)
            query = query.Where(filter);

        if (selector != null)
            return await query.Select(selector).FirstOrDefaultAsync(cancellationToken) ?? throw new InvalidOperationException("No entity found matching the criteria.");

        return (TResult)(object)await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetAsync<TInclude>(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, TInclude>? include = null, int pageSize = -1, int pageNumber = 1, CancellationToken cancellationToken = default) where TInclude : IQueryable<TEntity>
    {
        IQueryable<TEntity> query = _dbSet;

        if (include != null)
            query = include(query);

        if (filter != null)
            query = query.Where(filter);

        if (orderBy != null)
            query = orderBy(query);

        if (pageSize > 0)
        {
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken) ?? throw new InvalidOperationException("No entity found matching the criteria.");
    }

    public async Task<Application.Contracts.Persistence.Common.PagedResult<TResult>> GetPagedAsync<TResult, TInclude>(Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, TResult>>? selector = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, TInclude>? include = null, int pageSize = 10, int pageNumber = 1, CancellationToken cancellationToken = default) where TInclude : IQueryable<TEntity>
    {
        IQueryable<TEntity> query = _dbSet;

        if (include != null)
            query = include(query);

        if (filter != null)
            query = query.Where(filter);

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
            query = orderBy(query);

        var items = pageSize > 0
            ? query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
            : query;

        var results = selector != null
            ? await items.Select(selector).ToListAsync(cancellationToken)
            : (List<TResult>)(object)await items.ToListAsync(cancellationToken);

        return new Application.Contracts.Persistence.Common.PagedResult<TResult>
        {
            Data = results,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public IQueryable<TEntity> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public IQueryable<TEntity> Query<TInclude>(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, TInclude>? include = null) where TInclude : IQueryable<TEntity>
    {
        IQueryable<TEntity> query = _dbSet;

        if (include != null)
            query = include(query);

        if (filter != null)
            query = query.Where(filter);

        return query;
    }

    public IQueryable<TResult> Query<TResult>(Expression<Func<TEntity, bool>>? filter = null, Expression<Func<TEntity, TResult>>? selector = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (include != null)
            query = include(query);

        if (filter != null)
            query = query.Where(filter);

        if (selector != null)
            return query.Select(selector);

        return (IQueryable<TResult>)(object)query;
    }
}
