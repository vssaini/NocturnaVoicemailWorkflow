using System.Net;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using Polly;
using Refit;

namespace Nocturna.Infrastructure.Policies;

public class TokenApiPolicy
{
    /// <summary>
    /// Creates an HTTP retry policy that handles transient errors such as server errors (5xx), timeouts (408),
    /// and rate limit errors (429). The policy will retry the request up to 3 times using exponential backoff
    /// or custom delays based on the headers returned by the API, such as the Retry-After header.
    /// </summary>
    /// <param name="logger">The logger instance used for logging retry attempts, error details, and rate limit information.</param>
    /// <returns>
    /// An asynchronous retry policy that retries the HTTP request in the event of transient errors, such as server errors, timeouts, and rate limiting.
    /// </returns>
    public static AsyncRetryPolicy CreateHttpRetryPolicy(ILogger logger)
    {
        return Policy
            .Handle<ApiException>(ex =>
                (int)ex.StatusCode >= 500 ||
                ex.StatusCode == HttpStatusCode.RequestTimeout ||
                ex.StatusCode == HttpStatusCode.TooManyRequests
            )
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: (retryAttempt, exception, _) =>
                {
                    var apiEx = exception as ApiException;

                    if (apiEx?.StatusCode == HttpStatusCode.TooManyRequests)
                        return HandleRateLimitRetry(apiEx, retryAttempt, logger);

                    var delay = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt));
                    if (apiEx != null)
                    {
                        logger.LogWarning(apiEx, "[TOKEN API RETRY] Attempt {RetryAttempt} after {Delay}s due to API error {StatusCode}",
                            retryAttempt, delay.TotalSeconds, apiEx.StatusCode);
                    }
                    return delay;
                },
                onRetryAsync: async (exception, timespan, retryCount, _) =>
                {
                    if (exception is ApiException apiEx)
                    {
                        logger.LogWarning(apiEx, "[TOKEN API RETRY] Attempt {RetryCount} after {Delay}s due to API error {StatusCode}",
                            retryCount, timespan.TotalSeconds, apiEx.StatusCode);
                    }
                    await Task.CompletedTask;
                });
    }

    /// <summary>
    /// Handles rate limiting by checking the relevant API response headers, such as Retry-After and X-Rate-Limit.
    /// It calculates the appropriate delay based on the presence of these headers.
    /// </summary>
    /// <param name="apiEx">The ApiException containing the response headers from the API request.</param>
    /// <param name="retryAttempt">The current retry attempt number. This is used for exponential backoff if rate limit headers are not found.</param>
    /// <param name="logger">The logger instance used to log rate-limiting information and decisions.</param>
    /// <returns>
    /// A <see cref="TimeSpan"/> representing the amount of time to wait before retrying the request.
    /// </returns>
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
