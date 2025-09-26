namespace Persistence.Repositories;

internal class BulkRepository<TEntity> : IBulkRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public BulkRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }

    public async Task<BulkResult> BulkImportFromSourceAsync<TKey, TSource>(
        IEnumerable<TSource> sourceData,
        Expression<Func<TSource, TEntity>> mapper,
        Expression<Func<TEntity, TKey>>? keySelector = null,
        BulkInsertOptions<TEntity>? options = null,
        IProgress<BulkProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var compiledMapper = mapper.Compile();
            var sourceList = sourceData.ToList();
            var entities = sourceList.Select(compiledMapper).ToList();

            var result = await BulkInsertAsync(entities, keySelector, options, progress, cancellationToken);

            return result;
        }
        catch (Exception ex)
        {
            return new BulkResult
            {
                TotalRecords = 0,
                SuccessfulInserts = 0,
                ErroredRecords = 1,
                Errors = new List<BulkError> { new BulkError { ErrorMessage = ex.Message, Exception = ex } }
            };
        }
    }

    public async Task<BulkResult> BulkInsertAsync<TKey>(
        IEnumerable<TEntity> entities,
        Expression<Func<TEntity, TKey>>? keySelector = null,
        BulkInsertOptions<TEntity>? options = null,
        IProgress<BulkProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entityList = entities.ToList();
            var batchSize = options?.BatchSize ?? 1000;
            var totalCount = entityList.Count;
            var processedCount = 0;

            for (int i = 0; i < totalCount; i += batchSize)
            {
                var batch = entityList.Skip(i).Take(batchSize);
                await _dbSet.AddRangeAsync(batch, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                processedCount += batch.Count();

                progress?.Report(new BulkProgress
                {
                    ProcessedRecords = processedCount,
                    TotalRecords = totalCount,
                    SuccessfulRecords = processedCount
                });
            }

            return new BulkResult
            {
                TotalRecords = totalCount,
                SuccessfulInserts = totalCount,
                ErroredRecords = 0
            };
        }
        catch (Exception ex)
        {
            return new BulkResult
            {
                TotalRecords = 0,
                SuccessfulInserts = 0,
                ErroredRecords = 1,
                Errors = new List<BulkError> { new BulkError { ErrorMessage = ex.Message, Exception = ex } }
            };
        }
    }

    public async Task<BulkResult> BulkUpsertAsync<TKey>(
        IEnumerable<TEntity> entities,
            Expression<Func<TEntity, TKey>>? keySelector = null,
        Expression<Func<TEntity, TEntity, TEntity>>? updateExpression = null,
        BulkInsertOptions<TEntity>? options = null,
        IProgress<BulkProgress>? progress = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var entityList = entities.ToList();
            var batchSize = options?.BatchSize ?? 1000;
            var totalCount = entityList.Count;
            var processedCount = 0;
            var successfulInserts = 0;
            var updatedRecords = 0;

            if (keySelector == null)
            {
                // Simple bulk insert if no key selector provided
                return await BulkInsertAsync(entities, keySelector, options, progress, cancellationToken);
            }

            var compiledKeySelector = keySelector.Compile();
            var compiledUpdateExpression = updateExpression?.Compile();

            for (int i = 0; i < totalCount; i += batchSize)
            {
                var batch = entityList.Skip(i).Take(batchSize).ToList();
                var keys = batch.Select(compiledKeySelector).ToList();

                // Get existing entities
                var existingEntities = await _dbSet.Where(e => keys.Contains(compiledKeySelector(e)))
                    .ToListAsync(cancellationToken);

                var existingKeys = existingEntities.Select(compiledKeySelector).ToHashSet();
                var newEntities = batch.Where(e => !existingKeys.Contains(compiledKeySelector(e))).ToList();

                // Insert new entities
                if (newEntities.Any())
                {
                    await _dbSet.AddRangeAsync(newEntities, cancellationToken);
                    successfulInserts += newEntities.Count;
                }

                // Update existing entities
                if (compiledUpdateExpression != null && existingEntities.Any())
                {
                    foreach (var existing in existingEntities)
                    {
                        var batchEntity = batch.First(e => compiledKeySelector(e)!.Equals(compiledKeySelector(existing)));
                        var updated = compiledUpdateExpression(existing, batchEntity);
                        _context.Entry(existing).CurrentValues.SetValues(updated);
                        updatedRecords++;
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
                processedCount += batch.Count;

                progress?.Report(new BulkProgress
                {
                    ProcessedRecords = processedCount,
                    TotalRecords = totalCount,
                    SuccessfulRecords = successfulInserts + updatedRecords
                });
            }

            return new BulkResult
            {
                TotalRecords = totalCount,
                SuccessfulInserts = successfulInserts,
                UpdatedRecords = updatedRecords,
                ErroredRecords = 0
            };
        }
        catch (Exception ex)
        {
            return new BulkResult
            {
                TotalRecords = 0,
                SuccessfulInserts = 0,
                ErroredRecords = 1,
                Errors = new List<BulkError> { new BulkError { ErrorMessage = ex.Message, Exception = ex } }
            };
        }
    }
}