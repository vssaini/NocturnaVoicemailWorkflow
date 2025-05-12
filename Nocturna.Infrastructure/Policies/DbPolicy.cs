using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Nocturna.Infrastructure.Policies;

/// <summary>
/// Provides a retry policy for database operations using Polly.
/// </summary>
public static class DbPolicy
{
    /// <summary>
    /// Creates a default asynchronous retry policy for transient SQL or general exceptions.
    /// Retries up to 3 times with exponential backoff.
    /// </summary>
    /// <param name="logger">Logger to log retry attempts and exception details.</param>
    /// <returns>An asynchronous retry policy.</returns>
    public static AsyncRetryPolicy CreateDefaultRetryPolicy(ILogger logger)
    {
        return Policy
            .Handle<SqlException>(IsTransientSqlException)
            .Or<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, delay, retryCount, _) =>
                {
                    logger.LogError(
                        exception,
                        "[DB RETRY] Attempt {RetryAttempt} after {Delay}s due to error: {ErrorMessage}",
                        retryCount,
                        delay.TotalSeconds,
                        exception.Message);
                });
    }

    /// <summary>
    /// Determines whether a given SqlException is considered transient and eligible for retry.
    /// </summary>
    /// <param name="ex">The SqlException instance.</param>
    /// <returns>True if the error is transient; otherwise, false.</returns>
    private static bool IsTransientSqlException(SqlException ex)
    {
        foreach (SqlError error in ex.Errors)
        {
            if (TransientErrorNumbers.Contains(error.Number))
                return true;
        }

        return false;
    }

    /// <summary>
    /// A list of SQL error numbers considered transient.
    /// These can vary between SQL Server and Azure SQL.
    /// </summary>
    private static readonly int[] TransientErrorNumbers =
    [
        40613, // Database not available (Azure SQL)
        40197, // Azure SQL transient error
        40501, // Service busy
        49918, // Cannot process request. Too many operations
        49919, // Too many connections
        49920, // Too many operations in progress
        -2     // Timeout
    ];
}