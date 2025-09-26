using System.ComponentModel.DataAnnotations;

namespace Domain.Common;

/// <summary>
/// Base entity with audit fields for tracking creation, updates, and deletion
/// </summary>
/// <typeparam name="T">The type of the primary key</typeparam>
public abstract class AuditableEntity<T> : IStatusEntity
{
    [Key]
    public T Id { get; set; } = default!;

    // Creation tracking
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }

    // Update tracking
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Status and deletion tracking
    public Status StatusId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    // Business process tracking
    public DateTime? ProcessedAt { get; set; }
}