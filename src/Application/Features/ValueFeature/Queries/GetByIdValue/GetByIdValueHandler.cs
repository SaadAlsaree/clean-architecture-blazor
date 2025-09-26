using Application.Abstractions.Messaging.Query.GetById;
using Application.Features.ValueFeature.Queries.Shared;

namespace Application.Features.ValueFeature.Queries.GetByIdValue;

public class GetByIdValueHandler : GetByIdHandler<Value, GetByIdValueQuery, ValueViewModel>
{
    public GetByIdValueHandler(IReadRepository<Value> readRepository)
        : base(readRepository)
    {
    }

    protected override Expression<Func<Value, bool>>? FindPredicate(GetByIdValueQuery request) =>
        value => value.Id == request.Id && !value.IsDeleted;

    protected override ValueViewModel MapToViewModel(Value entity) =>
    new ValueViewModel
    {
        Id = entity.Id,
        Name = entity.Name,
        ValueNumber = entity.ValueNumber,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        StatusId = (int)entity.StatusId,
        StatusName = entity.StatusId.GetDisplayName()
    };


    protected override Expression<Func<Value, ValueViewModel>>? Selector(GetByIdValueQuery request)
    => value => new ValueViewModel
    {
        Id = value.Id,
        Name = value.Name,
        ValueNumber = value.ValueNumber,
        CreatedAt = value.CreatedAt,
        UpdatedAt = value.UpdatedAt,
        StatusId = (int)value.StatusId,
        StatusName = value.StatusId.GetDisplayName()
    };

}
