using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Nocturna.Presentation.Helpers;

/// <summary>
/// Provides utilities for parsing JSON payloads from <see cref="HttpRequestData"/>.
/// </summary>
public class JsonRequestParser
{
    /// <summary>
    /// Asynchronously reads and returns the raw JSON payload from the HTTP request body, 
    /// if the request has a readable body and a valid 'application/json' Content-Type header.
    /// </summary>
    /// <param name="req">The incoming HTTP request.</param>
    /// <param name="logger">An <see cref="ILogger"/> used to log warnings or errors during processing.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation if needed.</param>
    /// <returns>
    /// A <see cref="Task{String}"/> representing the asynchronous operation, with the raw JSON string if successful; 
    /// otherwise, <c>null</c> if the content type is not JSON or if reading fails.
    /// </returns>
    public static async Task<string?> ReadJsonBodyAsync(HttpRequestData req, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (req.Body is { CanRead: true } &&
            req.Headers.TryGetValues("Content-Type", out var ct) &&
            ct.Any(h => h.Contains("application/json", StringComparison.OrdinalIgnoreCase)))
            return await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);

        logger.LogWarning("Skipping body reading: Content-Type is not 'application/json'.");
        return null;
    }
}
