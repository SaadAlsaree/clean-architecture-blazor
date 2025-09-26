using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Application.ModelBinders;

/// <summary>
/// Custom model binder to ensure DateTime parameters from query strings are treated as UTC
/// </summary>
public class UtcDateTimeModelBinder : IModelBinder
{
    /// <summary>
    /// Binds the model to ensure DateTime values are treated as UTC
    /// </summary>
    /// <param name="bindingContext">The model binding context</param>
    /// <returns>A task representing the async operation</returns>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        if (DateTime.TryParse(value, out var dateTime))
        {
            // If the DateTime doesn't have a kind specified, treat it as UTC
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }
            // If it's Local, convert to UTC
            else if (dateTime.Kind == DateTimeKind.Local)
            {
                dateTime = dateTime.ToUniversalTime();
            }

            bindingContext.Result = ModelBindingResult.Success(dateTime);
            return Task.CompletedTask;
        }

        bindingContext.ModelState.TryAddModelError(modelName, "Invalid date format");
        bindingContext.Result = ModelBindingResult.Failed();
        return Task.CompletedTask;
    }
}

/// <summary>
/// Model binder provider for UTC DateTime binding
/// </summary>
public class UtcDateTimeModelBinderProvider : IModelBinderProvider
{
    /// <summary>
    /// Gets the model binder for DateTime types
    /// </summary>
    /// <param name="context">The model binder provider context</param>
    /// <returns>The model binder instance or null</returns>
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType == typeof(DateTime) ||
            context.Metadata.ModelType == typeof(DateTime?))
        {
            return new UtcDateTimeModelBinder();
        }

        return null;
    }
}
