using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Config;
using Nocturna.Domain.Models;

namespace Nocturna.Infrastructure.Services;

public class SecurityService(ILogger<SecurityService> logger, IOptions<RingCentralSettings> options) : ISecurityService
{
    public SecurityVerificationResult VerifyToken(HttpRequestData req)
    {
        if (req.Headers.TryGetValues("verification-token", out var tokens))
        {
            var token = tokens.First();
            if (token == options.Value.WebhookSecret)
                return new SecurityVerificationResult(true);

            logger.LogError("🛡️ Security verification failed — invalid verification-token {Token}", token);
            return new SecurityVerificationResult(false);
        }

        logger.LogError("🛡️ Security verification failed — verification-token header not found");
        return new SecurityVerificationResult(false);
    }
}