using System.ComponentModel.DataAnnotations;

namespace Domain.Common;

/// <summary>
/// Represents a structured error response
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// The error code
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// The error message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// The error category
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Additional details about the error
    /// </summary>
    public string? Details { get; set; }

    /// <summary>
    /// Timestamp when the error occurred
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Request ID for tracking
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Creates an error response from an ErrorsMessage enum
    /// </summary>
    /// <param name="errorMessage">The error message enum</param>
    /// <param name="details">Additional error details</param>
    /// <param name="requestId">Request ID for tracking</param>
    /// <returns>ErrorResponse object</returns>
    public static ErrorResponse Create(ErrorsMessage errorMessage, string? details = null, string? requestId = null)
    {
        var code = (int)errorMessage;
        var category = GetCategoryFromCode(code);

        return new ErrorResponse
        {
            Code = code,
            Message = ErrorHelper.GetDisplayName(errorMessage),
            Category = category,
            Details = details,
            RequestId = requestId
        };
    }

    /// <summary>
    /// Gets the category name from error code
    /// </summary>
    /// <param name="errorCode">The error code</param>
    /// <returns>Category name</returns>
    private static string GetCategoryFromCode(int errorCode)
    {
        return errorCode switch
        {
            >= 10000 and < 10100 => "عمليات قاعدة البيانات",
            >= 10100 and < 10200 => "أخطاء التحقق",
            >= 10200 and < 10300 => "أخطاء منطق العمل",
            >= 10300 and < 10400 => "المصادقة والتفويض",
            >= 10400 and < 10500 => "الخدمات الخارجية",
            >= 10500 and < 10600 => "عمليات الملفات",
            >= 10600 and < 10700 => "أخطاء النظام",
            _ => "غير محدد"
        };
    }

    /// <summary>
    /// Converts the error response to a dictionary
    /// </summary>
    /// <returns>Dictionary representation of the error</returns>
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            ["code"] = Code,
            ["message"] = Message,
            ["category"] = Category,
            ["timestamp"] = Timestamp,
            ["requestId"] = RequestId ?? string.Empty,
            ["details"] = Details ?? string.Empty
        };
    }
}
