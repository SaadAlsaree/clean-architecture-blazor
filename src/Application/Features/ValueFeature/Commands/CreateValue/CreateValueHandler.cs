namespace Application.Features.ValueFeature.Commands.CreateValue;

public class CreateValueHandler : CreateHandler<Value, CreateValueCommand, Guid>
{
    public CreateValueHandler(IWriteRepository<Value> repository, IReadRepository<Value> readRepository)
        : base(repository, readRepository)
    {
    }
    protected override Expression<Func<Value, bool>>? ExistencePredicate(CreateValueCommand request) =>
         value => value.Name == request.Name && !value.IsDeleted;

    protected override Value MapToEntity(CreateValueCommand request)
    => new Value
    {
        Id = Guid.NewGuid(),
        Name = request.Name,
        ValueNumber = request.ValueNumber,
        CreatedAt = DateTime.UtcNow,
        StatusId = Domain.Common.Status.Unverified
    };
    protected override Guid MapToResponse(Value entity) => entity.Id;

}
