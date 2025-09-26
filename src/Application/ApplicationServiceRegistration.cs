using Cortex.Mediator.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Reflection;


namespace Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplictionService(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddCortexMediator(
            configuration,
            new[] { typeof(ApplicationServiceRegistration) },
            options =>
            {
                options.AddDefaultBehaviors();
            }
        );


        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


        return services;
    }
}
