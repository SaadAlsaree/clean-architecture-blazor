using Application.Helper.Pagination;
using Application.Contracts.Persistence.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Messaging.Query.GetList;

/// <summary>
/// Abstract base handler for paginated list queries
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TQuery">The query type</typeparam>
/// <typeparam name="TViewModel">The view model type</typeparam>
public abstract class GetListWithPagedHandler<TEntity, TQuery, TViewModel>
    where TQuery : class, IQuery<Response<Application.Contracts.Persistence.Common.PagedResult<TViewModel>>>, IPaginationQuery
    where TEntity : class
{
    private readonly IReadRepository<TEntity> _readRepository;

    protected GetListWithPagedHandler(IReadRepository<TEntity> readRepository)
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
    protected virtual Task<Response<Application.Contracts.Persistence.Common.PagedResult<TViewModel>>> ValidateQuery(TQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Response<Application.Contracts.Persistence.Common.PagedResult<TViewModel>>.Success(default(Application.Contracts.Persistence.Common.PagedResult<TViewModel>)!));
    }

    /// <summary>
    /// Base handler method for paginated list queries
    /// </summary>
    protected async Task<Response<Application.Contracts.Persistence.Common.PagedResult<TViewModel>>> HandleBase(TQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            if (request == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(Application.Contracts.Persistence.Common.PagedResult<TViewModel>)!);

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

            // Get paginated results
            Application.Contracts.Persistence.Common.PagedResult<TViewModel> result;
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

                // Get total count
                var totalCount = await query.CountAsync(cancellationToken);

                // Get paginated data with projection
                var data = await query
                    .Select(selector)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync(cancellationToken);

                result = new Application.Contracts.Persistence.Common.PagedResult<TViewModel>
                {
                    Data = data,
                    TotalCount = totalCount,
                    PageNumber = request.Page,
                    PageSize = request.PageSize
                };
            }
            else
            {
                // Fallback to repository method with mapping
                var pagedResult = await _readRepository.GetPagedAsync<TViewModel, IQueryable<TEntity>>(
                    filter: filter,
                    selector: entity => MapToViewModel(entity),
                    orderBy: orderBy,
                    pageSize: request.PageSize,
                    pageNumber: request.Page,
                    cancellationToken: cancellationToken);

                if (pagedResult == null)
                    return ErrorsMessage.FailOnGet.ToErrorMessage(default(Application.Contracts.Persistence.Common.PagedResult<TViewModel>)!);

                result = new Application.Contracts.Persistence.Common.PagedResult<TViewModel>
                {
                    Data = pagedResult.Data,
                    TotalCount = pagedResult.TotalCount,
                    PageNumber = request.Page,
                    PageSize = request.PageSize
                };
            }

            return SuccessMessage.SuccessOnGet.ToSuccessMessage(result);
        }
        catch (Exception)
        {
            // Log the exception here if you have logging configured
            return ErrorsMessage.SystemError.ToErrorMessage(default(Application.Contracts.Persistence.Common.PagedResult<TViewModel>)!);
        }
    }
}
