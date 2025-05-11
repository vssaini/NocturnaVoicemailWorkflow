using Microsoft.Azure.Functions.Worker.Http;
using Nocturna.Application.Abstractions;

namespace Nocturna.Application.UseCases;

public class WebhookUseCase(
    IValidationTokenService validationService,
    ISecurityService securityService,
    IVoicemailService voicemailService)
    : IWebhookUseCase
{
    public async Task<HttpResponseData> ExecuteAsync(HttpRequestData req, string payload, CancellationToken cancellationToken = default)
    {
        // Step 1: Handle Validation Token (require during RingCentral Webhook setup)
        var validationResponse = validationService.HandleValidationToken(req, payload);
        if (validationResponse != null)
            return validationResponse;

        // Step 2: Verify developer defined validation token
        var verificationResult = await securityService.VerifyTokenAsync(req, cancellationToken);
        if (!verificationResult.IsValid)
            return verificationResult.Response;

        // Step 3: Process Voicemail Payload
        return await voicemailService.ProcessVoicemailAsync(req, payload, cancellationToken);
    }
}