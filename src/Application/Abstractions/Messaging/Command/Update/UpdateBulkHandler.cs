using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Messaging.Command.Update;

/// <summary>
/// Abstract base handler for bulk update operations using IBulkRepository
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TCommand">The command type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class UpdateBulkHandler<TEntity, TCommand, TResponse> : ICommandHandler<TCommand, Response<TResponse>>
    where TEntity : class
    where TCommand : ICommand<Response<TResponse>>
{
    private readonly IBulkRepository<TEntity> _bulkRepository;
    private readonly IReadRepository<TEntity> _readRepository;

    protected UpdateBulkHandler(IBulkRepository<TEntity> bulkRepository, IReadRepository<TEntity> readRepository)
    {
        _bulkRepository = bulkRepository ?? throw new ArgumentNullException(nameof(bulkRepository));
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
    }

    /// <summary>
    /// Defines the predicate to find entities to update
    /// </summary>
    protected abstract Expression<Func<TEntity, bool>>? FindPredicate(TCommand request);

    /// <summary>
    /// Maps command to collection of entities for update
    /// </summary>
    protected abstract IEnumerable<TEntity> MapToEntities(TCommand request, IEnumerable<TEntity> existingEntities);

    /// <summary>
    /// Gets the key selector for bulk operations
    /// </summary>
    protected abstract Expression<Func<TEntity, object>>? GetKeySelector();

    /// <summary>
    /// Updates the status for the entities
    /// </summary>
    protected virtual void UpdateStatus(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            if (entity is IStatusEntity statusEntity)
            {
                statusEntity.StatusId = Status.Verified;
            }
        }
    }

    /// <summary>
    /// Validates the entities before update
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
    /// Gets bulk update options
    /// </summary>
    protected virtual BulkInsertOptions<TEntity> GetBulkOptions()
    {
        return new BulkInsertOptions<TEntity>
        {
            BatchSize = 1000,
            UseTransaction = true,
            TimeoutInSeconds = 300,
            ConflictStrategy = ConflictResolutionStrategy.Update,
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
    /// Base handler method for bulk update operations
    /// </summary>
    protected async Task<Response<TResponse>> HandleBase(TCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            if (request == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            // Find entities to update
            var findPredicate = FindPredicate(request);
            if (findPredicate == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            var existingEntities = await _readRepository.GetQueryable()
                .Where(findPredicate)
                .ToListAsync(cancellationToken);
            if (existingEntities == null || !existingEntities.Any())
                return ErrorsMessage.NotExistOnUpdate.ToErrorMessage(default(TResponse)!);

            // Map command to entities for update
            var updatedEntities = MapToEntities(request, existingEntities);
            if (updatedEntities == null || !updatedEntities.Any())
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            // Update status
            UpdateStatus(updatedEntities);

            // Validate entities
            var validationResult = await ValidateEntities(updatedEntities, cancellationToken);
            if (!validationResult.Succeeded)
                return validationResult;

            // Get bulk options
            var options = GetBulkOptions();

            // Perform bulk upsert (update)
            var keySelector = GetKeySelector();
            var bulkResult = await _bulkRepository.BulkUpsertAsync(
                updatedEntities,
                keySelector,
                null, // updateExpression - null means update all fields
                options,
                null,
                cancellationToken);

            if (!bulkResult.IsSuccess)
                return ErrorsMessage.FailOnUpdate.ToErrorMessage(default(TResponse)!);

            // Map bulk result to response
            var responseData = MapToResponse(bulkResult);
            return SuccessMessage.SuccessOnUpdate.ToSuccessMessage(responseData);
        }
        catch (Exception)
        {
            // Log the exception here if you have logging configured
            return ErrorsMessage.SystemError.ToErrorMessage(default(TResponse)!);
        }
    }
}
