

namespace Persistence;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Npgsql to handle DateTime properly with UTC conversion
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", false);

        // Register the DbContext with the connection string from configuration
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Redis
        //services.AddStackExchangeRedisCache(options =>
        //{
        //    options.Configuration = configuration.GetConnectionString("RedisConnection");
        //});

        // Register repositories or other persistence-related services here
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        services.AddScoped(typeof(IExtensionRepository<>), typeof(ExtensionRepository<>));
        services.AddScoped(typeof(IBulkRepository<>), typeof(BulkRepository<>));
        services.AddScoped<ILoggedInUserService, LoggedInUserService>();
        services.AddScoped<IRedisCacheLayer, RedisCacheLayer>();
        services.AddScoped<IStorageService, StorageService>();
        return services;
    }
}
