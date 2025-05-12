using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Presentation.Helpers;

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

        var payload = await JsonRequestParser.ReadJsonBodyAsync(req, logger, cancellationToken);

        // Step 1: Handle Validation Token (require during RingCentral Webhook setup)
        var validationResponse = validationService.HandleValidationToken(req, payload);
        if (validationResponse != null)
            return validationResponse;

        // Step 2: Verify developer defined verification token
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
}