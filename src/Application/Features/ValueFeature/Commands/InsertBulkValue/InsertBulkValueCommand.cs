namespace Application.Features.ValueFeature.Commands.InsertBulkValue;

public class InsertBulkValueCommand : ICommand<Response<int>>
{
    public List<BulkValueItem> Values { get; set; } = new();
}

public class BulkValueItem
{
    public string Name { get; set; } = string.Empty;
    public int ValueNumber { get; set; }
}
