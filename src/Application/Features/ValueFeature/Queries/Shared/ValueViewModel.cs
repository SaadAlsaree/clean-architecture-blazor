namespace Application.Features.ValueFeature.Queries.Shared;

public class ValueViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ValueNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int StatusId { get; set; }
    public string StatusName { get; set; } = string.Empty;
}
