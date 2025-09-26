namespace Persistence.Repositories;

internal class WriteRepository<TEntity> : IWriteRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public WriteRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var result = await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return result.Entity;
    }

    public async Task<bool> CreateRangeAsync(IEnumerable<TEntity> entities, int batchSize = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            var entityList = entities.ToList();
            if (batchSize <= 0 || entityList.Count <= batchSize)
            {
                await _dbSet.AddRangeAsync(entityList, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                for (int i = 0; i < entityList.Count; i += batchSize)
                {
                    var batch = entityList.Skip(i).Take(batchSize);
                    await _dbSet.AddRangeAsync(batch, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _dbSet.Remove(entity);
            var result = await _context.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            var result = await _context.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteRangeAsync(IEnumerable<TEntity> entities, int batchSize = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            var entityList = entities.ToList();
            if (batchSize <= 0 || entityList.Count <= batchSize)
            {
                _dbSet.RemoveRange(entityList);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                for (int i = 0; i < entityList.Count; i += batchSize)
                {
                    var batch = entityList.Skip(i).Take(batchSize);
                    _dbSet.RemoveRange(batch);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<int> DeleteWhereAsync(Expression<Func<TEntity, bool>> filter, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(filter).ToListAsync(cancellationToken);
        _dbSet.RemoveRange(entities);
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            _dbSet.Update(entity);
            var result = await _context.SaveChangesAsync(cancellationToken);
            return result > 0;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities, int batchSize = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            var entityList = entities.ToList();
            if (batchSize <= 0 || entityList.Count <= batchSize)
            {
                _dbSet.UpdateRange(entityList);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                for (int i = 0; i < entityList.Count; i += batchSize)
                {
                    var batch = entityList.Skip(i).Take(batchSize);
                    _dbSet.UpdateRange(batch);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<int> UpdateWhereAsync(Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, TEntity>> updateExpression, CancellationToken cancellationToken = default)
    {
        var entities = await _dbSet.Where(filter).ToListAsync(cancellationToken);
        var compiledUpdate = updateExpression.Compile();

        foreach (var entity in entities)
        {
            var updatedEntity = compiledUpdate(entity);
            _context.Entry(entity).CurrentValues.SetValues(updatedEntity);
        }

        return await _context.SaveChangesAsync(cancellationToken);
    }
}
