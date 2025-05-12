using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;

namespace Nocturna.Presentation.Functions;

public class VoicemailWebhook(ILogger<VoicemailWebhook> logger, IValidationTokenService validationService, ISecurityService securityService)
{
    [Function(nameof(VoicemailWebhook))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Webhook received: RingCentral voicemail event");

        var payload = await GetPayloadFromRequestBodyAsync(req, cancellationToken);

        // Step 1: Handle Validation Token (require during RingCentral Webhook setup)
        var validationResponse = validationService.HandleValidationToken(req, payload);
        if (validationResponse != null)
            return validationResponse;

        // Step 2: Verify developer defined validation token
        var verificationResult = await securityService.VerifyTokenAsync(req, cancellationToken);
        if (!verificationResult.IsValid)
            return verificationResult.Response;

        // Step 3: Process voicemail payload
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(VoicemailWebhookOrchestrator), payload, cancellation: cancellationToken);
        logger.LogInformation("Started orchestration with ID = {InstanceId}", instanceId);

        var response = await client.CreateCheckStatusResponseAsync(req, instanceId, cancellation: cancellationToken);
        return response;
    }

    private async Task<string?> GetPayloadFromRequestBodyAsync(HttpRequestData req, CancellationToken cancellationToken)
    {
        if (req.Body is { CanRead: true } &&
             req.Headers.TryGetValues("Content-Type", out var ct) &&
             ct.Any(h => h.Contains("application/json", StringComparison.OrdinalIgnoreCase)))
        {
            try
            {
                return await new StreamReader(req.Body).ReadToEndAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to read the webhook request body.");
            }
        }
        else
        {
            logger.LogWarning("Skipping body reading: Content-Type is not 'application/json'.");
        }

        return null;
    }
}