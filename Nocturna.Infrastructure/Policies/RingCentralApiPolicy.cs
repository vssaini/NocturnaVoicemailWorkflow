using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Refit;
using System.Net;

namespace Nocturna.Infrastructure.Policies;

/// <summary>
/// Provides retry policies for external API calls, particularly for handling transient errors from services like RingCentral.
/// </summary>
public class RingCentralApiPolicy
{
    /// <summary>
    /// Creates an HTTP retry policy that handles transient errors, including server errors (5xx), timeouts (408), 
    /// and rate limit errors (429). The policy retries up to 3 times with exponential backoff or custom delays 
    /// based on the headers returned by the API.
    /// </summary>
    /// <param name="logger">Logger instance for logging retries and rate-limit information.</param>
    /// <returns>An asynchronous retry policy for HTTP requests.</returns>
    public static AsyncRetryPolicy CreateHttpRetryPolicy(ILogger logger)
    {
        return Policy
            .Handle<ApiException>(ex =>
                (int)ex.StatusCode >= 500 || // Server errors (5xx)
                ex.StatusCode == HttpStatusCode.RequestTimeout || // Timeout error (408)
                ex.StatusCode == HttpStatusCode.TooManyRequests   // Rate limit error (429)
            )
            .WaitAndRetryAsync(
                retryCount: 3, // Number of retry attempts
                sleepDurationProvider: (retryAttempt, exception, _) =>
                {
                    var apiEx = exception as ApiException;

                    if (apiEx?.StatusCode == HttpStatusCode.TooManyRequests)
                        return HandleRateLimitRetry(apiEx, retryAttempt, logger);

                    // For other transient errors, use exponential backoff
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    if (apiEx != null)
                    {
                        logger.LogWarning(apiEx, "[API RETRY] Attempt {RetryAttempt} after {Delay}s due to API error {StatusCode}",
                            retryAttempt, delay.TotalSeconds, apiEx.StatusCode);
                    }
                    return delay;
                },
                onRetryAsync: async (exception, timespan, retryCount, _) =>
                {
                    // Log retry attempts with the error details
                    if (exception is ApiException apiEx)
                    {
                        logger.LogWarning(apiEx, "[API RETRY] Attempt {RetryCount} after {Delay}s due to API error {StatusCode}",
                            retryCount, timespan.TotalSeconds, apiEx.StatusCode);
                    }
                    await Task.CompletedTask;
                });
    }

    /// <summary>
    /// Handles the rate-limiting scenario by checking relevant headers and determining the appropriate delay.
    /// </summary>
    /// <param name="apiEx">The ApiException containing the response headers.</param>
    /// <param name="retryAttempt">The current retry attempt number.</param>
    /// <param name="logger">Logger instance to log rate-limiting information.</param>
    /// <returns>A TimeSpan indicating how long to wait before retrying.</returns>
    private static TimeSpan HandleRateLimitRetry(ApiException apiEx, int retryAttempt, ILogger logger)
    {
        // If the Retry-After header is present, use it to determine the delay
        if (apiEx.Headers.TryGetValues("Retry-After", out var retryAfterValues) &&
            int.TryParse(retryAfterValues.FirstOrDefault(), out int retryAfterSeconds))
        {
            logger.LogWarning("[API RATE LIMIT] Retry-After header received. Waiting for {RetryAfterSeconds}s", retryAfterSeconds);
            return TimeSpan.FromSeconds(retryAfterSeconds);
        }

        // If X-Rate-Limit headers are present, use the remaining requests and window to calculate delay
        if (apiEx.Headers.TryGetValues("X-Rate-Limit-Remaining", out var remainingValues) &&
            apiEx.Headers.TryGetValues("X-Rate-Limit-Window", out var windowValues) &&
            int.TryParse(remainingValues.FirstOrDefault(), out int remainingRequests) &&
            int.TryParse(windowValues.FirstOrDefault(), out int windowDuration) &&
            remainingRequests == 0)
        {
            logger.LogWarning("[API RATE LIMIT] No remaining requests. Waiting for {WindowDuration}s", windowDuration);
            return TimeSpan.FromSeconds(windowDuration);
        }

        // Fallback: If no rate limit headers are found, use exponential backoff
        var fallbackDelay = TimeSpan.FromSeconds(30 * Math.Pow(2, retryAttempt));
        logger.LogWarning("[API RATE LIMIT] No rate limit headers found. Using exponential backoff: {Delay}s", fallbackDelay.TotalSeconds);
        return fallbackDelay;
    }

}