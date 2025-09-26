namespace Persistence.Repositories;

internal class ExtensionRepository<TEntity> : IExtensionRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _context;
    private bool _disposed = false;

    public ExtensionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ChangeStatusAsync<TStatus>(string tableName, TEntity primaryId, TStatus newStatus, CancellationToken cancellationToken = default)
    {
        try
        {
            var sql = $"UPDATE {tableName} SET StatusId = {{0}} WHERE Id = {{1}}";
            var result = await _context.Database.ExecuteSqlRawAsync(sql, newStatus!, primaryId, cancellationToken);
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<int> ChangeStatusBatchAsync<TStatus>(string tableName, IEnumerable<TEntity> primaryIds, TStatus newStatus, CancellationToken cancellationToken = default)
    {
        try
        {
            var ids = string.Join(",", primaryIds.Select(id => $"'{id}'"));
            var sql = $"UPDATE {tableName} SET StatusId = {{0}} WHERE Id IN ({ids})";
            return await _context.Database.ExecuteSqlRawAsync(sql, newStatus!, cancellationToken);
        }
        catch
        {
            return 0;
        }
    }

    public async Task<bool> DeleteRecordAsync(string tableName, TEntity primaryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sql = $"DELETE FROM {tableName} WHERE Id = {{0}}";
            var result = await _context.Database.ExecuteSqlRawAsync(sql, primaryId, cancellationToken);
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<int> DeleteRecordsAsync(string tableName, IEnumerable<TEntity> primaryIds, CancellationToken cancellationToken = default)
    {
        try
        {
            var ids = string.Join(",", primaryIds.Select(id => $"'{id}'"));
            var sql = $"DELETE FROM {tableName} WHERE Id IN ({ids})";
            return await _context.Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
        catch
        {
            return 0;
        }
    }

    public async Task<bool> RestoreRecordAsync(string tableName, TEntity primaryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sql = $"UPDATE {tableName} SET IsDeleted = false WHERE Id = {{0}}";
            var result = await _context.Database.ExecuteSqlRawAsync(sql, primaryId, cancellationToken);
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SoftDeleteRecordAsync(string tableName, TEntity primaryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sql = $"UPDATE {tableName} SET IsDeleted = true WHERE Id = {{0}}";
            var result = await _context.Database.ExecuteSqlRawAsync(sql, primaryId, cancellationToken);
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ToggleActiveStatusAsync(string tableName, TEntity primaryId, CancellationToken cancellationToken = default)
    {
        try
        {
            var sql = $"UPDATE {tableName} SET StatusId = CASE WHEN StatusId = 1 THEN 0 ELSE 1 END WHERE Id = {{0}}";
            var result = await _context.Database.ExecuteSqlRawAsync(sql, primaryId, cancellationToken);
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context?.Dispose();
            }
            _disposed = true;
        }
    }
}
