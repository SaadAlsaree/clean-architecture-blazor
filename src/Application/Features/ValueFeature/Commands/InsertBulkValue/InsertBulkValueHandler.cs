namespace Application.Features.ValueFeature.Commands.InsertBulkValue;

public class InsertBulkValueHandler : CreateBulkHandler<Value, InsertBulkValueCommand, int>
{
    public InsertBulkValueHandler(IBulkRepository<Value> bulkRepository, IReadRepository<Value> readRepository)
        : base(bulkRepository, readRepository)
    {
    }

    protected override Expression<Func<Value, bool>>? ExistencePredicate(InsertBulkValueCommand request) =>
        value => request.Values.Select(v => v.Name).Contains(value.Name) && !value.IsDeleted;

    protected override IEnumerable<Value> MapToEntities(InsertBulkValueCommand request)
    => request.Values.Select(item => new Value
    {
        Id = Guid.NewGuid(),
        Name = item.Name,
        ValueNumber = item.ValueNumber,
        CreatedAt = DateTime.UtcNow,
        StatusId = Domain.Common.Status.Unverified
    });


    protected override Expression<Func<Value, object>>? GetKeySelector() => value => value.Id;

    protected override int MapToResponse(BulkResult bulkResult) => bulkResult.SuccessfulInserts;
}
