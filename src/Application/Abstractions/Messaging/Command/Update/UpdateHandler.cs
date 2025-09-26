using Cortex.Mediator.Commands;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Messaging.Command.Update;

/// <summary>
/// Abstract base handler for update operations
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TCommand">The command type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class UpdateHandler<TEntity, TCommand, TResponse> : ICommandHandler<TCommand, Response<TResponse>>
    where TEntity : class
    where TCommand : ICommand<Response<TResponse>>
{
    private readonly IWriteRepository<TEntity> _repository;
    private readonly IReadRepository<TEntity> _readRepository;

    protected UpdateHandler(IWriteRepository<TEntity> repository, IReadRepository<TEntity> readRepository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
    }

    /// <summary>
    /// Defines the predicate to find the entity to update
    /// </summary>
    protected abstract Expression<Func<TEntity, bool>>? FindPredicate(TCommand request);

    /// <summary>
    /// Maps command to entity for update
    /// </summary>
    protected abstract TEntity MapToEntity(TCommand request, TEntity existingEntity);

    /// <summary>
    /// Updates the status for the entity
    /// </summary>
    protected virtual void UpdateStatus(TEntity entity)
    {
        if (entity is IStatusEntity statusEntity)
        {
            statusEntity.StatusId = Status.Verified;
        }
    }

    /// <summary>
    /// Validates the entity before update
    /// </summary>
    protected virtual Task<Response<TResponse>> ValidateEntity(TEntity entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(Response<TResponse>.Success(default(TResponse)!));
    }

    /// <summary>
    /// Maps the updated entity to response data
    /// </summary>
    protected abstract TResponse MapToResponse(TEntity entity);

    /// <summary>
    /// Handle method implementation for ICommandHandler
    /// </summary>
    public async Task<Response<TResponse>> Handle(TCommand command, CancellationToken cancellationToken = default)
    {
        return await HandleBase(command, cancellationToken);
    }

    /// <summary>
    /// Base handler method for update operations
    /// </summary>
    protected async Task<Response<TResponse>> HandleBase(TCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            if (request == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            // Find the entity to update
            var findPredicate = FindPredicate(request);
            if (findPredicate == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            var existingEntity = await _readRepository.GetQueryable()
                .Where(findPredicate)
                .FirstOrDefaultAsync(cancellationToken);
            if (existingEntity == null)
                return ErrorsMessage.NotExistOnUpdate.ToErrorMessage(default(TResponse)!);

            // Map command to entity for update
            var updatedEntity = MapToEntity(request, existingEntity);
            if (updatedEntity == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            // Update status
            UpdateStatus(updatedEntity);

            // Validate entity
            var validationResult = await ValidateEntity(updatedEntity, cancellationToken);
            if (!validationResult.Succeeded)
                return validationResult;

            // Update entity
            var result = await _repository.UpdateAsync(updatedEntity, cancellationToken);

            if (!result)
                return ErrorsMessage.FailOnUpdate.ToErrorMessage(default(TResponse)!);

            // Map entity to response
            var responseData = MapToResponse(updatedEntity);
            return SuccessMessage.SuccessOnUpdate.ToSuccessMessage(responseData);
        }
        catch (Exception)
        {
            // Log the exception here if you have logging configured
            return ErrorsMessage.SystemError.ToErrorMessage(default(TResponse)!);
        }
    }
}
