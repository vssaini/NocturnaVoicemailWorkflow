using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Refit;

namespace Nocturna.Infrastructure.Policies;

/// <summary>
/// Provides retry policies for external API calls, particularly for handling transient errors from services like RingCentral.
/// </summary>
public class ApiPollyPolicy
{
    /// <summary>
    /// Creates a retry policy that handles transient HTTP failures such as 5xx server errors,
    /// timeouts, and rate limiting (429).
    /// </summary>
    /// <param name="logger">Logger used to record retry attempts and exception details.</param>
    /// <returns>An asynchronous retry policy configured for API error scenarios.</returns>
    public static AsyncRetryPolicy CreateHttpRetryPolicy(ILogger logger)
    {
        return Policy
            .Handle<ApiException>(ex =>
                    (int)ex.StatusCode >= 500 || // Server errors
                    ex.StatusCode == System.Net.HttpStatusCode.RequestTimeout || // 408
                    ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests   // 429
            )
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timespan, retryCount, _) =>
                {
                    var apiEx = (ApiException)exception;
                    logger.LogError(
                        apiEx,
                        "[API RETRY] Attempt {RetryCount} after {Delay}s due to API error {StatusCode}",
                        retryCount,
                        timespan.TotalSeconds,
                        apiEx.StatusCode);
                });
    }
}