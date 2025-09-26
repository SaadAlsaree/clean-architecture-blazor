using Application;
using Application.ModelBinders;
using FormCraft;
using FormCraft.ForMudBlazor.Extensions;
using MudBlazor.Services;
using Persistence;
using WebApp.Components;
using WebApp.Services;
using WebApp.ViewModels;


namespace WebApp;

/// <summary>
/// Extension methods for configuring services and pipeline
/// </summary>
public static class StartupExtensions
{
    /// <summary>
    /// Configures application services
    /// </summary>
    /// <param name="builder">The web application builder</param>
    /// <returns>The configured web application</returns>
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorService();
        builder.Services.AddApplictionService(builder.Configuration);
        builder.Services.AddPersistenceServices(builder.Configuration);



        // Add MudBlazor services
        builder.Services.AddMudServices();
        builder.Services.AddFormCraft();
        builder.Services.AddFormCraftMudBlazor();


        builder.Services.AddScoped<ThemeSettings>();
        builder.Services.AddScoped<IValueManagementService, ValueManagementService>();
        builder.Services.AddScoped<ValueManagementViewModel>();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents()
            .AddInteractiveWebAssemblyComponents();

        // Add API controllers with custom DateTime model binder
        builder.Services.AddControllers(options =>
        {
            // Add UTC DateTime model binder to handle DateTime parameters properly
            options.ModelBinderProviders.Insert(0, new UtcDateTimeModelBinderProvider());
        })
        .AddJsonOptions(options =>
        {
            // Configure JSON serialization for consistent DateTime handling
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

        // Add Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();


        return builder.Build();
    }

    /// <summary>
    /// Configures the application pipeline
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The configured web application</returns>
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline.


        app.UseHttpsRedirection();


        app.UseAntiforgery();

        app.MapStaticAssets();

        // Map API controllers
        app.MapControllers();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(WebApp.Client._Imports).Assembly);

        return app;
    }
}
