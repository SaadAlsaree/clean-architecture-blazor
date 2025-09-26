namespace Application.Contracts.Infrastructure;

public interface ICsvExporter
{
    byte[] ExportEventsToCsv(List<Type> eventExportDtos);
}