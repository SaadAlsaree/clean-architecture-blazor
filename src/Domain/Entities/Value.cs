using Domain.Common;

namespace Domain.Entities;

public class Value : IStatusEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ValueNumber { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
    public Status StatusId { get; set; } = Status.Unverified;
}
