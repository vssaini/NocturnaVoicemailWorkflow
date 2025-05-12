using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Nocturna.Presentation.Extensions;

/// <summary>
/// Extension methods to register services for the function app.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds function services including Application Insights and logging services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> to read the settings from.</param>
    public static void AddFunctionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationInsights();
        services.AddLogging(configuration);
    }

    private static void AddApplicationInsights(this IServiceCollection services)
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // The Application Insights SDK adds a default logging filter that instructs ILogger to capture only Warning and more severe logs.
        // Application Insights requires an explicit override.
        // Log levels can also be configured using appsettings.json. For more information, see https://learn.microsoft.com/en-us/azure/azure-monitor/app/worker-service#ilogger-logs
        services.Configure<LoggerFilterOptions>(options =>
        {
            LoggerFilterRule? defaultRule = options.Rules.FirstOrDefault(rule => rule.ProviderName
                == "Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider");
            if (defaultRule is not null)
            {
                options.Rules.Remove(defaultRule);
            }
        });
    }

    private static void AddLogging(this IServiceCollection services, IConfiguration configuration)
    {
        // Set up Serilog with configuration from appsettings.json
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders(); // Remove the default logging providers
            loggingBuilder.AddSerilog();

            // Disable IHttpClientFactory Informational logs. Source: https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=hostbuilder%2Cwindows#logging
            // Note -- you can also remove the handler that does the logging: https://github.com/aspnet/HttpClientFactory/issues/196#issuecomment-432755765 
            loggingBuilder.AddFilter("System.Net.Http.HttpClient", LogLevel.Warning);
        });

        // To find out why Serilog is not logging to specific sink, enable self logging.
        //Serilog.Debugging.SelfLog.Enable(Console.Error);
    }
}
