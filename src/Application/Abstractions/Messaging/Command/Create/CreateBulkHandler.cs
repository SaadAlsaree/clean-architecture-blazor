using Cortex.Mediator.Commands;

namespace Application.Abstractions.Messaging.Command.Create;

/// <summary>
/// Abstract base handler for bulk create operations using IBulkRepository
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TCommand">The command type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class CreateBulkHandler<TEntity, TCommand, TResponse> : ICommandHandler<TCommand, Response<TResponse>>
    where TEntity : class
    where TCommand : ICommand<Response<TResponse>>
{
    private readonly IBulkRepository<TEntity> _bulkRepository;
    private readonly IReadRepository<TEntity> _readRepository;

    protected CreateBulkHandler(IBulkRepository<TEntity> bulkRepository, IReadRepository<TEntity> readRepository)
    {
        _bulkRepository = bulkRepository ?? throw new ArgumentNullException(nameof(bulkRepository));
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
    }

    /// <summary>
    /// Defines the predicate to check if entities already exist
    /// </summary>
    protected abstract Expression<Func<TEntity, bool>>? ExistencePredicate(TCommand request);

    /// <summary>
    /// Maps command to collection of entities
    /// </summary>
    protected abstract IEnumerable<TEntity> MapToEntities(TCommand request);

    /// <summary>
    /// Gets the key selector for bulk operations
    /// </summary>
    protected abstract Expression<Func<TEntity, object>>? GetKeySelector();

    /// <summary>
    /// Sets the initial status for the entities
    /// </summary>
    protected virtual void SetInitialStatus(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            if (entity is IStatusEntity statusEntity)
            {
                statusEntity.StatusId = Status.Unverified;
            }
        }
    }

    /// <summary>
    /// Validates the entities before creation
    /// </summary>
    protected virtual Task<Response<TResponse>> ValidateEntities(IEnumerable<TEntity> entities, CancellationToken cancellationToken)
    {
        return Task.FromResult(Response<TResponse>.Success(default(TResponse)!));
    }

    /// <summary>
    /// Maps the bulk result to response data
    /// </summary>
    protected abstract TResponse MapToResponse(BulkResult bulkResult);

    /// <summary>
    /// Gets bulk insert options
    /// </summary>
    protected virtual BulkInsertOptions<TEntity> GetBulkOptions()
    {
        return new BulkInsertOptions<TEntity>
        {
            BatchSize = 1000,
            UseTransaction = true,
            TimeoutInSeconds = 300,
            ConflictStrategy = ConflictResolutionStrategy.Skip,
            ValidateEntities = true,
            ErrorHandling = BulkErrorHandling.ContinueOnError,
            ReturnDetailedResults = true
        };
    }

    /// <summary>
    /// Handle method implementation for ICommandHandler
    /// </summary>
    public async Task<Response<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        return await HandleBase(command, cancellationToken);
    }

    /// <summary>
    /// Base handler method for bulk create operations
    /// </summary>
    protected async Task<Response<TResponse>> HandleBase(TCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            if (request == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            // Map command to entities
            var entities = MapToEntities(request);
            if (entities == null || !entities.Any())
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            // Check if any entities already exist
            var existencePredicate = ExistencePredicate(request);
            if (existencePredicate != null)
            {
                var exists = await _readRepository.ExistsAsync(existencePredicate, cancellationToken);
                if (exists)
                    return ErrorsMessage.ExistOnCreate.ToErrorMessage(default(TResponse)!);
            }

            // Set initial status
            SetInitialStatus(entities);

            // Validate entities
            var validationResult = await ValidateEntities(entities, cancellationToken);
            if (!validationResult.Succeeded)
                return validationResult;

            // Get bulk options
            var options = GetBulkOptions();

            // Perform bulk insert
            var keySelector = GetKeySelector();
            var bulkResult = await _bulkRepository.BulkInsertAsync(
                entities,
                keySelector,
                options,
                null,
                cancellationToken);

            if (!bulkResult.IsSuccess)
                return ErrorsMessage.FailOnCreate.ToErrorMessage(default(TResponse)!);

            // Map bulk result to response
            var responseData = MapToResponse(bulkResult);
            return SuccessMessage.SuccessOnCreate.ToSuccessMessage(responseData);
        }
        catch (Exception)
        {
            // Log the exception here if you have logging configured
            return ErrorsMessage.SystemError.ToErrorMessage(default(TResponse)!);
        }
    }
}
