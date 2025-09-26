namespace Application.Features.ValueFeature.Queries.ExportCsvValue;

public class ExportCsvValueQuery : IQuery<Response<string>>
{
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}
