using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Enums;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Presentation.Functions.Activities;
using Nocturna.Presentation.Helpers;

namespace Nocturna.Presentation.Functions;

public class VoicemailWebhookOrchestrator
{
    [Function(nameof(VoicemailWebhookOrchestrator))]
    public static async Task<RequestResult> Run([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        WebhookPayloadDto? payloadDto = null;
        var logger = context.CreateReplaySafeLogger<VoicemailWebhookOrchestrator>();

        try
        {
            var payload = context.GetInput<string>()!;

            payloadDto = await ParseAndValidatePayload(context, payload);
            if (payloadDto == null)
                return Error("Invalid payload");

            var payloadDbId = await SavePayloadToDatabase(context, payloadDto, payload);

            var attachmentId = await FetchAttachmentId(context, payloadDto);
            if (attachmentId is null)
                return Error("Transcription attachment Id not found");

            var transcription = await FetchTranscriptionText(context, payloadDto, attachmentId.Value);
            if (string.IsNullOrWhiteSpace(transcription))
                return Error("Failed to fetch transcription");

            await SaveTranscriptionAndWriteToFtp(context, payloadDto, transcription, payloadDbId);

            return RequestResult.Create(RequestStatus.Success, "Transcription saved successfully", transcription);
        }
        catch (Exception ex)
        {
            context.SetCustomStatus($"Step: Error - {ex.Message}");

            var uuid = payloadDto?.Uuid ?? "unknown";
            var errorMessage = $"[VoicemailWebhookOrchestrator] Error during orchestration for UUID: {uuid}. Message: {ex.Message}";

            logger.LogError(ex, errorMessage);
            return Error("Internal server error while processing voicemail webhook");
        }
    }

    private static async Task<WebhookPayloadDto?> ParseAndValidatePayload(TaskOrchestrationContext context, string payload)
    {
        context.SetCustomStatus("Step: ParsePayload - Validating webhook payload");
        return await context.CallActivityAsync<WebhookPayloadDto?>(nameof(ParsePayload), payload);
    }

    private static async Task<int> SavePayloadToDatabase(TaskOrchestrationContext context, WebhookPayloadDto dto, string rawPayload)
    {
        context.SetCustomStatus("Step: SavePayload - Persisting payload to database");
        var activityContext = new ActivityContext<string>(dto.Uuid, rawPayload);
        return await context.CallActivityAsync<int>(nameof(SavePayload), activityContext);
    }

    private static async Task<long?> FetchAttachmentId(TaskOrchestrationContext context, WebhookPayloadDto dto)
    {
        context.SetCustomStatus("Step: FetchAttachmentId - Getting transcription attachment ID");
        var attachContext = new ActivityContext<WebhookPayloadDto>(dto.Uuid, dto);
        return await AttachmentHandler.FetchAttachmentIdAsync(context, attachContext);
    }

    private static async Task<string> FetchTranscriptionText(TaskOrchestrationContext context, WebhookPayloadDto dto, long attachmentId)
    {
        context.SetCustomStatus("Step: FetchTranscription - Fetching transcription text");
        var transInput = new FetchTransInput(dto, attachmentId);
        var transContext = new ActivityContext<FetchTransInput>(dto.Uuid, transInput);
        return await context.CallActivityAsync<string>(nameof(FetchTranscription), transContext);
    }

    private static async Task SaveTranscriptionAndWriteToFtp(TaskOrchestrationContext context, WebhookPayloadDto dto, string transcription, int payloadDbId)
    {
        context.SetCustomStatus("Step: SaveTranscription - Saving transcription and writing to FTP");
        var input = new TranscriptionInput(dto, transcription, payloadDbId);
        var activityContext = new ActivityContext<TranscriptionInput>(dto.Uuid, input);

        await context.CallActivityAsync(nameof(SaveTranscription), activityContext);
        await context.CallActivityAsync(nameof(WriteTranscriptionToFtp), activityContext);
    }

    private static RequestResult Error(string message) =>
        RequestResult.Create(RequestStatus.Error, message);
}

