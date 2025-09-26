using Application.Features.ValueFeature.Queries.Shared;

namespace Application.Features.ValueFeature.Queries.GetByIdValue;

public class GetByIdValueQuery : IQuery<Response<ValueViewModel>>
{
    public Guid Id { get; set; }
}
