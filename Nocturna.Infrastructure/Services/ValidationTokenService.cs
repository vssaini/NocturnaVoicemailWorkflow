using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using System.Net;

namespace Nocturna.Infrastructure.Services;

public class ValidationTokenService(ILogger<ValidationTokenService> logger) : IValidationTokenService
{
    public HttpResponseData? HandleValidationToken(HttpRequestData req, string payload)
    {
        logger.LogDebug("Checking for Validation-Token header in request: {Method} {Url}", req.Method, req.Url);

        if (req.Headers.TryGetValues("Validation-Token", out var validationTokens) && string.IsNullOrWhiteSpace(payload))
        {
            var token = validationTokens.FirstOrDefault();
            logger.LogInformation("Handling webhook endpoint setup validation. Echoing token: {Token}", token ?? "<null>");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Validation-Token", token);
            logger.LogInformation("Returning 200 OK for webhook validation.");
            return response;
        }

        logger.LogDebug("Validation-Token header not present or body not empty; continuing with normal webhook processing.");
        return null; // Continue with normal processing
    }
}