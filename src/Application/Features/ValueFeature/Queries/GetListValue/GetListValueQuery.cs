using Application.Features.ValueFeature.Queries.Shared;
using Application.Helper.Pagination;


namespace Application.Features.ValueFeature.Queries.GetListValue;

public class GetListValueQuery : IQuery<Response<Application.Contracts.Persistence.Common.PagedResult<ValueViewModel>>>, IPaginationQuery
{
    public string? Name { get; set; }
    public int? StatusId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public int Page { get; set; } = 1;
    public byte PageSize { get; set; } = 10;
}
