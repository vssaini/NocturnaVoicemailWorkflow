using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using System.Net;

namespace Nocturna.Infrastructure.Services;

public class ValidationTokenService(ILogger<ValidationTokenService> logger) : IValidationTokenService
{
    public HttpResponseData? HandleValidationToken(HttpRequestData req)
    {
        if (!req.Headers.TryGetValues("Validation-Token", out var validationTokens))
            return null; // Continue with normal processing

        var token = validationTokens.FirstOrDefault();
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Validation-Token", token);

        logger.LogInformation("🤝 RingCentral validation request acknowledged (200 OK).");
        return response;
    }
}