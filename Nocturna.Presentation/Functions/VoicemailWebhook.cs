using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using System.Net;
using Nocturna.Domain.Models;
using DocumentFormat.OpenXml.Drawing;
using System.Threading;

namespace Nocturna.Presentation.Functions;

public class VoicemailWebhook(IWebhookUseCase webhookUseCase, ILogger<VoicemailWebhook> logger)
{
    [Function(nameof(VoicemailWebhook))]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        CancellationToken cancellationToken)
    {
        // Note: STARTER FUNCTION

        logger.LogInformation("Webhook received: RingCentral voicemail event");

        var webhookInput = await GetWebhookInputAsync(req, cancellationToken);
        var instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(VoicemailWebhookOrchestrator), webhookInput, cancellation: cancellationToken);

        var response = await client.WaitForCompletionOrCreateCheckStatusResponseAsync(
            req,
            instanceId);

        //var result = await client
        //    .ScheduleNewOrchestrationInstanceAsync(nameof(VoicemailWebhookOrchestrator), webhookInput, cancellation: cancellationToken);


        //if (result?.IsValidationRequest == true && !string.IsNullOrEmpty(result.ValidationToken))
        //{
        //    var response = req.CreateResponse(HttpStatusCode.OK);
        //    response.Headers.Add("Validation-Token", result.ValidationToken);
        //    logger.LogInformation("Returning 200 OK for webhook validation.");
        //    return response;
        //}

        //// Continue with regular webhook response
        //var defaultResponse = req.CreateResponse(HttpStatusCode.Accepted);
        //await defaultResponse.WriteStringAsync("Webhook received.");
        //return defaultResponse;

        //logger.LogInformation("Started orchestration with ID = {InstanceId}", instanceId);

        //var response = await client.CreateCheckStatusResponseAsync(req, instanceId, cancellation: cancellationToken);
        return response;

       // return await webhookUseCase.ExecuteAsync(req, payload, cancellationToken); // TODO: Fix
    }

    private async Task<WebhookInput> GetWebhookInputAsync(HttpRequestData req, CancellationToken cancellationToken)
    {
        var payload = await GetPayloadFromRequestBodyAsync(req, cancellationToken);
        var headers = req.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);

        return new WebhookInput
        {
            Payload = payload,
            Headers = headers
        };
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