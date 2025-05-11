#nullable enable

using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Nocturna.Infrastructure.Logging;

public static class LoggerExtensions
{
    /// <summary>
    /// Handles adding extra properties to the LogContext before executing the log action.
    /// </summary>
    private static void LogWithProperties(Action logAction, Dictionary<string, object?>? extraProperties)
    {
        var disposables = new List<IDisposable>();

        try
        {
            // Add extra properties to LogContext
            if (extraProperties != null)
            {
                foreach (var (key, value) in extraProperties)
                {
                    disposables.Add(LogContext.PushProperty(key, value));
                }
            }

            // Invoke actual logging method
            logAction();
        }
        finally
        {
            // Ensure all disposable properties are removed after logging
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }

    /// <summary>
    /// Custom method for logging informational messages.
    /// </summary>
    public static void LogInfo<T>(this ILogger<T> logger, string messageTemplate, params object[] propertyValues)
    {
        logger.LogInformation(messageTemplate, propertyValues);
    }

    /// <summary>
    /// Custom method for logging informational messages with additional properties.
    /// </summary>
    public static void LogInfo<T>(this ILogger<T> logger, string messageTemplate, Dictionary<string, object?>? extraProperties, params object[] propertyValues)
    {
        LogWithProperties(() => logger.LogInformation(messageTemplate, propertyValues), extraProperties);
    }

    /// <summary>
    /// Custom method for logging warnings.
    /// </summary>
    public static void LogWarn<T>(this ILogger<T> logger, string messageTemplate, params object[] propertyValues)
    {
        logger.LogWarning(messageTemplate, propertyValues);
    }

    /// <summary>
    /// Custom method for logging warnings with additional properties.
    /// </summary>
    public static void LogWarn<T>(this ILogger<T> logger, string messageTemplate, Dictionary<string, object?>? extraProperties, params object[] propertyValues)
    {
        LogWithProperties(() => logger.LogWarning(messageTemplate, propertyValues), extraProperties);
    }

    /// <summary>
    /// Custom method for logging errors.
    /// </summary>
    public static void LogErr<T>(this ILogger<T> logger, Exception exc, string messageTemplate, params object[] propertyValues)
    {
        exc = GetInnermostException(exc);
        logger.LogError(exc, messageTemplate, propertyValues);
    }

    /// <summary>
    /// Custom method for logging errors with additional properties.
    /// </summary>
    public static void LogErr<T>(this ILogger<T> logger, Exception exc, string messageTemplate, Dictionary<string, object?>? extraProperties, params object[] propertyValues)
    {
        exc = GetInnermostException(exc);
        LogWithProperties(() => logger.LogError(exc, messageTemplate, propertyValues), extraProperties);
    }

    /// <summary>
    /// Custom method for logging error message with additional properties.
    /// </summary>
    public static void LogErr<T>(this ILogger<T> logger, string messageTemplate, Dictionary<string, object?>? extraProperties, params object[] propertyValues)
    {
        LogWithProperties(() => logger.LogError(messageTemplate, propertyValues), extraProperties);
    }

    /// <summary>
    /// Custom method for logging debug messages.
    /// </summary>
    public static void LogDbg<T>(this ILogger<T> logger, string messageTemplate, params object[] propertyValues)
    {
        logger.LogDebug(messageTemplate, propertyValues);
    }

    /// <summary>
    /// Custom method for logging debug messages with additional properties.
    /// </summary>
    public static void LogDbg<T>(this ILogger<T> logger, string messageTemplate, Dictionary<string, object?>? extraProperties, params object[] propertyValues)
    {
        LogWithProperties(() => logger.LogDebug(messageTemplate, propertyValues), extraProperties);
    }

    /// <summary>
    /// Custom method for logging verbose (trace) messages.
    /// </summary>
    public static void LogVerb<T>(this ILogger<T> logger, string messageTemplate, params object[] propertyValues)
    {
        logger.LogTrace(messageTemplate, propertyValues);
    }

    /// <summary>
    /// Custom method for logging verbose (trace) messages with additional properties.
    /// </summary>
    public static void LogVerb<T>(this ILogger<T> logger, string messageTemplate, Dictionary<string, object?>? extraProperties, params object[] propertyValues)
    {
        LogWithProperties(() => logger.LogTrace(messageTemplate, propertyValues), extraProperties);
    }

    /// <summary>
    /// Retrieves the innermost exception in the exception chain.
    /// </summary>
    private static Exception GetInnermostException(Exception exc)
    {
        while (exc.InnerException != null)
        {
            exc = exc.InnerException;
        }
        return exc;
    }
}
