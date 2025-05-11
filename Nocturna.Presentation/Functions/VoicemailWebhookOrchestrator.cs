using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Models;
using Nocturna.Presentation.Functions.Activities;

namespace Nocturna.Presentation.Functions;

public static class VoicemailWebhookOrchestrator
{
    [Function(nameof(VoicemailWebhookOrchestrator))]
    public static async Task<RequestResult> Run(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var webhookInput = context.GetInput<WebhookInput>()!;

        var result = await context.CallActivityAsync<RequestResult>(nameof(ValidateToken), webhookInput);
        if (result.IsValidationRequest && !string.IsNullOrEmpty(result.ValidationToken))
        {
            // Means RingCentral is setting up Webhook; and so we must return
            return result;
        }

        await context.CallActivityAsync<RequestResult>(nameof(VerifyToken), webhookInput);

        result = await context.CallActivityAsync<RequestResult>("ProcessVoicemail", webhookInput);
        return result;
    }

    [Function(nameof(SayHello))]
    public static string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("SayHello");
        logger.LogInformation("Saying hello to {name}.", name);
        return $"Hello {name}!";
    }

    [Function("Function_HttpStart")]
    public static async Task<HttpResponseData> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
        [DurableClient] DurableTaskClient client,
        FunctionContext executionContext)
    {
        ILogger logger = executionContext.GetLogger("Function_HttpStart");

        // Function input comes from the request content.
        string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
            nameof(VoicemailWebhookOrchestrator));

        logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

        // Returns an HTTP 202 response with an instance management payload.
        // See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
        return await client.CreateCheckStatusResponseAsync(req, instanceId);
    }
}