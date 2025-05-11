using Nocturna.Application.Constants;
using Serilog;

namespace Nocturna.Presentation.Helpers;

/// <summary>
/// Provides functionality to log application startup events exactly once per host process initialization.
/// </summary>
internal static class StartupLogger
{
    private static bool _hasLoggedStartup;

    /// <summary>
    /// Logs the application startup message if it has not been logged before in the current host process.
    /// </summary>
    public static void LogStartup()
    {
        if (_hasLoggedStartup)
            return;

        var logger = Log.ForContext("SourceContext", AppConstants.SourceContext);
        logger.Information("Azure Function '{AppName}' is starting on {MachineName}.", AppConstants.AppName, Environment.MachineName);

        _hasLoggedStartup = true;
    }
}