using Microsoft.Azure.Functions.Worker;
using Microsoft.DurableTask;
using Nocturna.Domain.Enums;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Presentation.Functions.Activities;
using Nocturna.Presentation.Helpers;

namespace Nocturna.Presentation.Functions;

public static class VoicemailWebhookOrchestrator
{
    [Function(nameof(VoicemailWebhookOrchestrator))]
    public static async Task<RequestResult> Run(
        [OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var payload = context.GetInput<string>()!;

        var dbPayloadId = await context.CallActivityAsync<int>(nameof(SavePayload), payload);
        var payloadDto = await context.CallActivityAsync<WebhookPayloadDto>(nameof(ParsePayload), payload);

        var isValid = await context.CallActivityAsync<bool>(nameof(ValidatePayload), payloadDto);
        if (!isValid)
            return RequestResult.Create(RequestStatus.Error, "Invalid payload");

        var isVoicemail = await context.CallActivityAsync<bool>(nameof(IsVoicemailEvent), payloadDto);
        if (!isVoicemail)
            return RequestResult.Create(RequestStatus.Error, "Invalid event type");

        var attachmentId = await AttachmentRetryHandler.FetchWithRetryAsync(context, payloadDto);
        if (attachmentId is null)
            return RequestResult.Create(RequestStatus.Error, "Transcription attachment ID not found after retries");

        var voicemailMsg = new VoicemailMessage(payloadDto.Body.Id, attachmentId.Value);
        var transcription = await context.CallActivityAsync<string>(nameof(FetchTranscription), voicemailMsg);
        if (string.IsNullOrWhiteSpace(transcription))
            return RequestResult.Create(RequestStatus.Error, "Failed to fetch transcription");

        var transInput = new TranscriptionInput(payloadDto, transcription, dbPayloadId);
        await context.CallActivityAsync(nameof(SaveTranscription), transInput);

        await context.CallActivityAsync("WriteTranscriptionToFtpActivity", transInput);

        return RequestResult.Create(RequestStatus.Success, "Transcription saved successfully!", transcription);
    }
}