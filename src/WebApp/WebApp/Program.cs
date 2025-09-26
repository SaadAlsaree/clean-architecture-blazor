
using WebApp;

//Log.Logger = new LoggerConfiguration()
//    .WriteTo.Console()
//    .CreateBootstrapLogger();

//Log.Information("Analyzer Starting up!");

var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
//     .WriteTo.Console()
//     .ReadFrom.Configuration(context.Configuration));

builder.Services.AddLogging();

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

//app.UseSerilogRequestLogging();

//await app.ResetDatabaseAsync();

// Seed the database with SuperAdmin data
//await app.SeedSuperAdminAsync();


app.Run();