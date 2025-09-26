using Application.Features.ValueFeature.Commands.CreateValue;
using FluentValidation;

namespace WebApp;

public static class ValidatorServiceRegistration
{
    public static IServiceCollection AddValidatorService(this IServiceCollection services)
    {
        // Register FluentValidation
        services.AddScoped<IValidator<CreateValueCommand>, CreateValueValidator>();

        return services;
    }
}
