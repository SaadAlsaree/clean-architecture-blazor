using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Messaging.Query.GetList;

public abstract class GetListHandler<TEntity, TQuery, TViewModel> : IQueryHandler<TQuery, Response<IEnumerable<TViewModel>>>
    where TQuery : IQuery<Response<IEnumerable<TViewModel>>>
    where TEntity : class
{
    private readonly IReadRepository<TEntity> _readRepository;

    protected GetListHandler(IReadRepository<TEntity> readRepository)
    {
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
    }

    /// <summary>
    /// Defines the filter predicate for the query
    /// </summary>
    protected abstract Expression<Func<TEntity, bool>>? FilterPredicate(TQuery request);

    /// <summary>
    /// Defines the ordering for the query
    /// </summary>
    protected abstract Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? OrderBy(TQuery request);

    /// <summary>
    /// Maps entity to view model
    /// </summary>
    protected abstract TViewModel MapToViewModel(TEntity entity);

    /// <summary>
    /// Maps entities to view models
    /// </summary>
    protected virtual IEnumerable<TViewModel> MapToViewModels(IEnumerable<TEntity> entities)
    {
        return entities.Select(MapToViewModel);
    }

    /// <summary>
    /// Defines the selector for projection to view model
    /// </summary>
    protected virtual Expression<Func<TEntity, TViewModel>>? Selector(TQuery request)
    {
        return null;
    }

    /// <summary>
    /// Validates the query before execution
    /// </summary>
    protected virtual Task<Response<IEnumerable<TViewModel>>> ValidateQuery(TQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Response<IEnumerable<TViewModel>>.Success(default(IEnumerable<TViewModel>)!));
    }

    /// <summary>
    /// Handle method implementation for IRequestHandler
    /// </summary>
    public async Task<Response<IEnumerable<TViewModel>>> Handle(TQuery request, CancellationToken cancellationToken = default)
    {
        return await HandleBase(request, cancellationToken);
    }

    /// <summary>
    /// Base handler method for list queries
    /// </summary>
    protected async Task<Response<IEnumerable<TViewModel>>> HandleBase(TQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            if (request == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(IEnumerable<TViewModel>)!);

            // Validate query
            var validationResult = await ValidateQuery(request, cancellationToken);
            if (!validationResult.Succeeded)
                return validationResult;

            // Get filter predicate
            var filter = FilterPredicate(request);

            // Get ordering
            var orderBy = OrderBy(request);

            // Get selector expression
            var selector = Selector(request);

            // Get all data with projection or mapping
            IEnumerable<TViewModel> viewModels;
            if (selector != null)
            {
                // Use projection for better performance
                var query = _readRepository.GetQueryable();
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                if (orderBy != null)
                {
                    query = orderBy(query);
                }
                viewModels = await query.Select(selector).ToListAsync(cancellationToken);
            }
            else
            {
                // Fallback to entity then mapping
                var entities = await _readRepository.GetAsync<IQueryable<TEntity>>(
                    filter: filter,
                    orderBy: orderBy,
                    pageSize: -1, // -1 means get all data without pagination
                    pageNumber: 1,
                    cancellationToken: cancellationToken);

                if (entities == null)
                    return ErrorsMessage.FailOnGet.ToErrorMessage(default(IEnumerable<TViewModel>)!);

                // Map entities to view models
                viewModels = MapToViewModels(entities);
            }

            return SuccessMessage.SuccessOnGet.ToSuccessMessage(viewModels);
        }
        catch (Exception)
        {
            // Log the exception here if you have logging configured
            return ErrorsMessage.SystemError.ToErrorMessage(default(IEnumerable<TViewModel>)!);
        }
    }
}
