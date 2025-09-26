using Application.Abstractions.Messaging.Query.GetList;
using Application.Contracts.Persistence.Common;
using Application.Features.ValueFeature.Queries.Shared;

namespace Application.Features.ValueFeature.Queries.GetListValue;

public class GetListValueHandler : GetListWithPagedHandler<Value, GetListValueQuery, ValueViewModel>, IQueryHandler<GetListValueQuery, Response<PagedResult<ValueViewModel>>>
{
    public GetListValueHandler(IReadRepository<Value> readRepository)
        : base(readRepository)
    {
    }

    public async Task<Response<Application.Contracts.Persistence.Common.PagedResult<ValueViewModel>>> Handle(GetListValueQuery request, CancellationToken cancellationToken)
    {
        return await HandleBase(request, cancellationToken);
    }

    protected override Expression<Func<Value, bool>>? FilterPredicate(GetListValueQuery request)
    {
        // Convert DateTime parameters to UTC to avoid PostgreSQL timezone issues
        var createdFromUtc = request.CreatedFrom?.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.CreatedFrom.Value, DateTimeKind.Utc)
            : request.CreatedFrom?.ToUniversalTime();

        var createdToUtc = request.CreatedTo?.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(request.CreatedTo.Value, DateTimeKind.Utc)
            : request.CreatedTo?.ToUniversalTime();

        return value => !value.IsDeleted &&
            (string.IsNullOrEmpty(request.Name) || value.Name.Contains(request.Name)) &&
            (!request.StatusId.HasValue || (int)value.StatusId == request.StatusId) &&
            (!createdFromUtc.HasValue || value.CreatedAt >= createdFromUtc) &&
            (!createdToUtc.HasValue || value.CreatedAt <= createdToUtc);
    }


    protected override Func<IQueryable<Value>, IOrderedQueryable<Value>>? OrderBy(GetListValueQuery request) =>
        query => query.OrderByDescending(v => v.CreatedAt);


    protected override ValueViewModel MapToViewModel(Value entity)
    => new ValueViewModel
    {
        Id = entity.Id,
        Name = entity.Name,
        ValueNumber = entity.ValueNumber,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt,
        StatusId = (int)entity.StatusId,
        StatusName = entity.StatusId.GetDisplayName()
    };


    protected override Expression<Func<Value, ValueViewModel>>? Selector(GetListValueQuery request)
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
