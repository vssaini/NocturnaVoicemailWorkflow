using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nocturna.Application.Abstractions;
using System.Net;
using Nocturna.Domain.Models;
using Nocturna.Domain.Config;

namespace Nocturna.Infrastructure.Services;

public class SecurityService(ILogger<SecurityService> logger, IOptions<RingCentralSettings> options) : ISecurityService
{
    public async Task<SecurityVerificationResult> VerifyTokenAsync(HttpRequestData req, CancellationToken cancellationToken = default)
    {
        if (req.Headers.TryGetValues("validation-token", out var tokens))
        {
            var token = tokens.First();
            if (token != options.Value.WebhookSecret)
            {
                logger.LogWarning("Security verification failed — invalid validation-token {Token}", token);
                var response = await CreateErrorResponseAsync(req, "Invalid validation-token", cancellationToken);
                return new SecurityVerificationResult { IsValid = false, Response = response };
            }

            logger.LogInformation("Security verification completed — validation-token matched");
            var successResponse = req.CreateResponse(HttpStatusCode.OK);
            return new SecurityVerificationResult { IsValid = true, Response = successResponse };
        }

        logger.LogWarning("Security verification failed — validation-token header not found");
        var missingResponse = await CreateErrorResponseAsync(req, "Required header validation-token is missing", cancellationToken);
        return new SecurityVerificationResult { IsValid = false, Response = missingResponse };
    }

    /// <summary>
    /// Creates a 401 Unauthorized response with a descriptive error message.
    /// Used when the developer-defined validation-token is missing or invalid.
    /// </summary>
    /// <param name="req">The incoming HTTP request.</param>
    /// <param name="message">The error message to include in the response body.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>An HTTP 401 Unauthorized response containing the error details.</returns>
    private static async Task<HttpResponseData> CreateErrorResponseAsync(HttpRequestData req, string message, CancellationToken cancellationToken)
    {
        var response = req.CreateResponse(HttpStatusCode.Unauthorized);
        await response.WriteAsJsonAsync(new { status = "error", message }, cancellationToken: cancellationToken);
        return response;
    }
}