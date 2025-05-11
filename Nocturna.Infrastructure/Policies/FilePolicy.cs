using FluentFTP.Exceptions;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Nocturna.Infrastructure.Policies;

/// <summary>
/// Provides retry policies for file transfer operations like uploading to FTP servers using Polly.
/// </summary>
public static class FilePolicy
{
    /// <summary>
    /// Creates a retry policy for handling transient FTP errors such as permission issues (e.g., 550 errors),
    /// I/O issues, and other unexpected failures during file upload or save operations.
    /// </summary>
    /// <param name="logger">Logger used to record retry attempts and error details.</param>
    /// <returns>An asynchronous retry policy for FTP upload operations.</returns>
    public static AsyncRetryPolicy CreateFtpRetryPolicy(ILogger logger)
    {
        return Policy
            .Handle<FtpCommandException>(ex => ex.CompletionCode == "550") // Access denied
            .Or<IOException>() // File system errors
            .Or<UnauthorizedAccessException>() // File permission issues
            .Or<Exception>() // Any unhandled exception
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timespan, retryCount, _) =>
                {
                    logger.LogError(
                        exception,
                        "[FTP RETRY] Attempt {RetryCount} after {Delay}s due to: {ErrorType} - {ErrorMessage}",
                        retryCount,
                        timespan.TotalSeconds,
                        exception.GetType().Name,
                        exception.Message);
                });
    }
}