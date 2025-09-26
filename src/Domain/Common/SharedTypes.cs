using System.ComponentModel.DataAnnotations;

namespace Domain.Common;

/// <summary>
/// Base interface for request handlers
/// </summary>


/// <summary>
/// Status enumeration for entities
/// </summary>
public enum Status
{
    [Display(Name = "غير مفعل")]
    Unverified = 0,
    [Display(Name = "مفعل")]
    Verified = 1,
    [Display(Name = "نشط")]
    Active = 2,
    [Display(Name = "غير نشط")]
    Inactive = 3,
    [Display(Name = "محذوف")]
    Deleted = 4
}

/// <summary>
/// Base interface for entities with status
/// </summary>
public interface IStatusEntity
{
    Status StatusId { get; set; }
}
