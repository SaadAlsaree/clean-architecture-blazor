using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Messaging.Query.GetById;

public abstract class GetByIdHandler<TEntity, TQuery, TViewModel> : IQueryHandler<TQuery, Response<TViewModel>>
    where TQuery : IQuery<Response<TViewModel>>
    where TEntity : class
{
    private readonly IReadRepository<TEntity> _readRepository;

    protected GetByIdHandler(IReadRepository<TEntity> readRepository)
    {
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
    }

    /// <summary>
    /// Defines the filter predicate for finding the entity
    /// </summary>
    protected abstract Expression<Func<TEntity, bool>>? FindPredicate(TQuery request);

    /// <summary>
    /// Maps entity to view model
    /// </summary>
    protected abstract TViewModel MapToViewModel(TEntity entity);

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
    protected virtual Task<Response<TViewModel>> ValidateQuery(TQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(Response<TViewModel>.Success(default(TViewModel)!));
    }

    /// <summary>
    /// Handle method implementation for IRequestHandler
    /// </summary>
    public async Task<Response<TViewModel>> Handle(TQuery request, CancellationToken cancellationToken = default)
    {
        return await HandleBase(request, cancellationToken);
    }

    /// <summary>
    /// Base handler method for getting entity by ID
    /// </summary>
    protected async Task<Response<TViewModel>> HandleBase(TQuery request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            if (request == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TViewModel)!);

            // Validate query
            var validationResult = await ValidateQuery(request, cancellationToken);
            if (!validationResult.Succeeded)
                return validationResult;

            // Get filter predicate
            var filter = FindPredicate(request);

            // Get selector expression
            var selector = Selector(request);

            // Find the entity with projection
            TViewModel viewModel;
            if (selector != null)
            {
                // Use projection for better performance
                var query = _readRepository.GetQueryable();
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                viewModel = await query.Select(selector).FirstOrDefaultAsync(cancellationToken) ?? default(TViewModel)!;
            }
            else
            {
                // Fallback to entity then mapping
                var query = _readRepository.GetQueryable();
                if (filter != null)
                {
                    query = query.Where(filter);
                }
                var entity = await query.FirstOrDefaultAsync(cancellationToken);

                if (entity == null)
                    return ErrorsMessage.FailOnGet.ToErrorMessage(default(TViewModel)!);

                // Map entity to view model
                viewModel = MapToViewModel(entity);
            }

            if (viewModel == null)
                return ErrorsMessage.FailOnGet.ToErrorMessage(default(TViewModel)!);

            return SuccessMessage.SuccessOnGet.ToSuccessMessage(viewModel);
        }
        catch (Exception)
        {
            // Log the exception here if you have logging configured
            return ErrorsMessage.SystemError.ToErrorMessage(default(TViewModel)!);
        }
    }
}
