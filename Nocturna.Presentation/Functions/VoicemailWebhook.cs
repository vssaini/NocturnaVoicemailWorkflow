using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Presentation.Helpers;
using System.Net;

namespace Nocturna.Presentation.Functions;

public class VoicemailWebhook(ILogger<VoicemailWebhook> logger, IValidationTokenService validationService, ISecurityService securityService)
{
    [Function(nameof(VoicemailWebhook))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("🔔 Webhook received from RingCentral");

        // Step 1: Handle validation token (require during initial RingCentral Webhook setup)
        var validationResponse = validationService.HandleValidationToken(req);
        if (validationResponse != null)
            return validationResponse;

        // Step 2: Respond immediately with 200 OK to prevent RingCentral timeout.
        // If delayed, the Azure Function may return status code 499 (Client Closed Request),
        // with an error like:
        // System.Runtime.InteropServices.COMException:
        // Exception while executing function: Functions.VoicemailWebhook
        // Exception binding parameter 'req': The client has disconnected.
        // An operation was attempted on a nonexistent network connection. (0x800704CD)

        var ackResponse = req.CreateResponse(HttpStatusCode.OK);
        await ackResponse.WriteStringAsync("OK", cancellationToken);

        // Step 3: Run all heavy/slow logic in background
        _ = HandleWebhookInBackgroundAsync(req, client, context.InvocationId, cancellationToken);

        return ackResponse;
    }

    private async Task HandleWebhookInBackgroundAsync(HttpRequestData req,
        DurableTaskClient client,
        string invocationId,
        CancellationToken cancellationToken)
    {
        try
        {
            var verificationResult = securityService.VerifyToken(req);
            if (!verificationResult.IsValid)
                return;

            var payload = await JsonRequestParser.ReadJsonBodyAsync(req, logger, cancellationToken);
            if (payload == null)
                return;

            var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
                nameof(VoicemailWebhookOrchestrator), payload, cancellation: cancellationToken);
            logger.LogInformation("🎼 Started orchestration {InstanceId}", instanceId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❗ Error in {Method}. InvocationId: {InvocationId}, Method: {HttpMethod}, Path: {Path}. Exception: {Message}",
                nameof(HandleWebhookInBackgroundAsync),
                invocationId,
                req.Method,
                req.Url.AbsolutePath,
                ex.Message);
        }
    }
}