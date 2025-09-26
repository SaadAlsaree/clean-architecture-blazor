using Cortex.Mediator.Commands;

namespace Application.Abstractions.Messaging.Command.Create;

/// <summary>
/// Abstract base handler for create operations
/// </summary>
/// <typeparam name="TEntity">The entity type</typeparam>
/// <typeparam name="TCommand">The command type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public abstract class CreateHandler<TEntity, TCommand, TResponse> : ICommandHandler<TCommand, Response<TResponse>>
    where TEntity : class
    where TCommand : ICommand<Response<TResponse>>
{
    private readonly IWriteRepository<TEntity> _repository;
    private readonly IReadRepository<TEntity> _readRepository;

    protected CreateHandler(IWriteRepository<TEntity> repository, IReadRepository<TEntity> readRepository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _readRepository = readRepository ?? throw new ArgumentNullException(nameof(readRepository));
    }

    /// <summary>
    /// Defines the predicate to check if entity already exists
    /// </summary>
    protected abstract Expression<Func<TEntity, bool>>? ExistencePredicate(TCommand request);

    /// <summary>
    /// Maps command to entity
    /// </summary>
    protected abstract TEntity MapToEntity(TCommand request);

    /// <summary>
    /// Sets the initial status for the entity
    /// </summary>
    protected virtual void SetInitialStatus(TEntity entity)
    {
        if (entity is IStatusEntity statusEntity)
        {
            statusEntity.StatusId = Status.Unverified;
        }
    }

    /// <summary>
    /// Validates the entity before creation
    /// </summary>
    protected virtual Task<Response<TResponse>> ValidateEntity(TEntity entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(Response<TResponse>.Success(default(TResponse)!));
    }

    /// <summary>
    /// Maps the created entity to response data
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
    /// Base handler method for create operations
    /// </summary>
    protected async Task<Response<TResponse>> HandleBase(TCommand request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate request
            if (request == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            // Check if entity already exists
            var existencePredicate = ExistencePredicate(request);
            if (existencePredicate != null)
            {
                var exists = await _readRepository.ExistsAsync(existencePredicate, cancellationToken);
                if (exists)
                    return ErrorsMessage.ExistOnCreate.ToErrorMessage(default(TResponse)!);
            }

            // Map command to entity
            var entity = MapToEntity(request);
            if (entity == null)
                return ErrorsMessage.InvalidInputData.ToErrorMessage(default(TResponse)!);

            // Set initial status
            SetInitialStatus(entity);

            // Validate entity
            var validationResult = await ValidateEntity(entity, cancellationToken);
            if (!validationResult.Succeeded)
                return validationResult;

            // Create entity
            var createdEntity = await _repository.CreateAsync(entity, cancellationToken);

            if (createdEntity == null)
                return ErrorsMessage.FailOnCreate.ToErrorMessage(default(TResponse)!);

            // Map entity to response
            var responseData = MapToResponse(createdEntity);
            return SuccessMessage.SuccessOnCreate.ToSuccessMessage(responseData);
        }
        catch (Exception)
        {
            // Log the exception here if you have logging configured
            return ErrorsMessage.SystemError.ToErrorMessage(default(TResponse)!);
        }
    }
}