using Application.Abstractions.Messaging.Command.Update;

namespace Application.Features.ValueFeature.Commands.UpdateValue;

public class UpdateValueHandler : UpdateHandler<Value, UpdateValueCommand, Guid>
{
    public UpdateValueHandler(IWriteRepository<Value> repository, IReadRepository<Value> readRepository)
        : base(repository, readRepository)
    {
    }

    protected override Expression<Func<Value, bool>>? FindPredicate(UpdateValueCommand request) =>
        value => value.Id == request.Id && !value.IsDeleted;

    protected override Value MapToEntity(UpdateValueCommand request, Value existingEntity)
    {
        existingEntity.Name = request.Name;
        existingEntity.ValueNumber = request.ValueNumber;
        existingEntity.UpdatedAt = DateTime.UtcNow;
        return existingEntity;
    }

    protected override Guid MapToResponse(Value entity) => entity.Id;
}
