namespace Application.Features.ValueFeature.Queries.ExportExcelValue;

public class ExportExcelValueQuery : IQuery<Response<string>>
{

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
