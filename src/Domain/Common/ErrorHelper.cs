using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Domain.Common;

/// <summary>
/// Helper class for working with error messages
/// </summary>
public static class ErrorHelper
{
    /// <summary>
    /// Gets the display name for an error message enum value
    /// </summary>
    /// <param name="errorMessage">The error message enum value</param>
    /// <returns>The display name or the enum name if no display attribute is found</returns>
    public static string GetDisplayName(ErrorsMessage errorMessage)
    {
        var field = errorMessage.GetType().GetField(errorMessage.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.Name ?? errorMessage.ToString();
    }

    /// <summary>
    /// Gets the error code for an error message enum value
    /// </summary>
    /// <param name="errorMessage">The error message enum value</param>
    /// <returns>The integer value of the enum</returns>
    public static int GetErrorCode(ErrorsMessage errorMessage)
    {
        return (int)errorMessage;
    }

    /// <summary>
    /// Gets error messages by category
    /// </summary>
    /// <param name="category">The category to filter by</param>
    /// <returns>Dictionary of error codes and their display names</returns>
    public static Dictionary<int, string> GetErrorsByCategory(ErrorCategory category)
    {
        var errors = new Dictionary<int, string>();
        var minCode = (int)category;
        var maxCode = minCode + 99;

        foreach (ErrorsMessage error in Enum.GetValues<ErrorsMessage>())
        {
            var code = (int)error;
            if (code >= minCode && code <= maxCode)
            {
                errors[code] = GetDisplayName(error);
            }
        }

        return errors;
    }

    /// <summary>
    /// Checks if an error code belongs to a specific category
    /// </summary>
    /// <param name="errorCode">The error code to check</param>
    /// <param name="category">The category to check against</param>
    /// <returns>True if the error code belongs to the category</returns>
    public static bool IsErrorInCategory(int errorCode, ErrorCategory category)
    {
        var minCode = (int)category;
        var maxCode = minCode + 99;
        return errorCode >= minCode && errorCode <= maxCode;
    }
}

/// <summary>
/// Error categories for organizing error messages
/// </summary>
public enum ErrorCategory
{
    DatabaseOperations = 10000,
    ValidationErrors = 10100,
    BusinessLogicErrors = 10200,
    AuthenticationAuthorization = 10300,
    ExternalServices = 10400,
    FileOperations = 10500,
    SystemErrors = 10600
}
