using System.Diagnostics;
using CustomTemplate.Installers;
using Serilog;
using Serilog.Events;

try
{
    // Logging Configuration 

    var configuration = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(configuration)
            .WriteTo.Console()
        .CreateLogger();

    Serilog.Debugging.SelfLog.Enable(msg =>
    {
        Debug.Print(msg);
        Debugger.Break();
    });

    // App building settings
    
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();
    builder.Services.InstallServices(builder.Configuration);

    var app = builder.Build();
    app.InstallExtensions();
}
catch (Exception e)
{
    Log.Fatal($"Host terminated unexpectedly: {e.Message}");
}
finally
{
    Log.CloseAndFlush();
}
