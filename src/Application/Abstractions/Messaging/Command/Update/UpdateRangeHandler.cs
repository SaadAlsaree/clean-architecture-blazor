using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Messaging.Command.Update;

/// <summary>
/// Abstract base handler for bulk update operations
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TCommand">The command type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class UpdateRangeHandler<TEntity, TCommand, TResponse> : ICommandHandler<TCommand, Response<TResponse>>
    where TEntity : class
    where TCommand : ICommand<Response<TResponse>>
{
    private readonly IWriteRepository<TEntity> _repository;
    private readonly IReadRepository<TEntity> _readRepository;

    protected UpdateRangeHandler(IWriteRepository<TEntity> repository, IReadRepository<TEntity> readRepository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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
    /// Maps the updated entities to response data
    /// </summary>
    protected abstract TResponse MapToResponse(IEnumerable<TEntity> entities);

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

            // Update entities in bulk
            var success = await _repository.UpdateRangeAsync(updatedEntities, cancellationToken: cancellationToken);

            if (!success)
                return ErrorsMessage.FailOnUpdate.ToErrorMessage(default(TResponse)!);

            // Map entities to response
            var responseData = MapToResponse(updatedEntities);
            return SuccessMessage.SuccessOnUpdate.ToSuccessMessage(responseData);
        }
        catch (Exception)
        {
            // Log the exception here if you have logging configured
            return ErrorsMessage.SystemError.ToErrorMessage(default(TResponse)!);
        }
    }
}
