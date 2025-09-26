using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Messaging.Query.ExportFile;

public abstract class ExportCsvHandler<TEntity, TQuery>
    where TQuery : class
    where TEntity : class
{
    private readonly IReadRepository<TEntity> _readRepository;

    protected ExportCsvHandler(IReadRepository<TEntity> readRepository)
    {
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
    }

    public abstract Expression<Func<TEntity, List<object>>> Selector { get; }
    public abstract Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? Include { get; }
    public abstract string[] Headers { get; }

    /// <summary>
    /// Defines the filter predicate for the query
    /// </summary>
    protected abstract Expression<Func<TEntity, bool>>? FilterPredicate(TQuery request);

    public async Task<byte[]> HandleBase(TQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var query = _readRepository.GetQueryable();

            // Apply filter
            var filter = FilterPredicate(request);
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (Include != null)
            {
                query = Include(query);
            }

            var result = await query
                .Select(Selector)
                .ToListAsync(cancellationToken: cancellationToken);

            if (!result.Any())
                return null!;

            var file = Utilities.GetFileCsv(result, Headers);
            return file;
        }
        catch (Exception)
        {
            return null!;
        }
    }
}
