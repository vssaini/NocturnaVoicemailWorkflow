using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Abstractions;
using Nocturna.Domain.Entities;
using Nocturna.Domain.Enums;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Infrastructure.Policies;

namespace Nocturna.Infrastructure.Services;

public class VoicemailProcessor(
    ITranscriptFetcher transcriptFetcher,
    IVoicemailRepository voicemailRepository,
    ILogger<VoicemailProcessor> logger)
    : IVoicemailProcessor
{
    public async Task<int> SavePayloadAsync(ActivityContext<string> context, CancellationToken cancellationToken = default)
    {
        var dbRetryPolicy = DbPolicy.CreateDefaultRetryPolicy(context.PayloadUuid, logger);

        var payloadId = await dbRetryPolicy.ExecuteAsync(async ct =>
            await voicemailRepository.SaveWebhookPayloadAsync(context, ct), cancellationToken);

        return payloadId;
    }

    public async Task<string> GetTranscriptionAsync(ActivityContext<VoicemailMessage> context, CancellationToken cancellationToken = default)
    {
        var voicemailMessage = context.Data;
        var request = new TranscriptionRequest(
            voicemailMessage.AccountId,
            voicemailMessage.ExtensionId,
            voicemailMessage.MessageId,
            voicemailMessage.AttachmentId,
            ContentDisposition.Inline);
        var transContext = new ActivityContext<TranscriptionRequest>(context.PayloadUuid, request);
        var transcription = await transcriptFetcher.GetTranscriptionAsync(transContext, cancellationToken);

        if (string.IsNullOrWhiteSpace(transcription))
        {
            logger.LogWarning("Payload {PayloadUuid} - Transcription for message {MessageId} (attachment {AttachmentId}) is empty or null.", context.PayloadUuid, voicemailMessage.MessageId, voicemailMessage.AttachmentId);
            return string.Empty;
        }

        logger.LogDebug("Payload {PayloadUuid} - Audio transcription for message {MessageId} (attachment {AttachmentId}): {Transcription}", context.PayloadUuid, voicemailMessage.MessageId, voicemailMessage.AttachmentId, transcription);
        return transcription;

    }

    public async Task SaveVoicemailTranscriptionAsync(ActivityContext<TranscriptionInput> context, CancellationToken cancellationToken = default)
    {
        var vmTranscription = MapPayloadToTranscription(context.Data.Payload, context.Data.Transcription);
        var transContext = new ActivityContext<VoicemailTranscription>(context.PayloadUuid, vmTranscription);

        var dbRetryPolicy = DbPolicy.CreateDefaultRetryPolicy(context.PayloadUuid, logger);

        await dbRetryPolicy.ExecuteAsync(async ct =>
            await voicemailRepository.SaveVoicemailTranscriptionAsync(context.Data.SavedPayloadId, transContext, ct), cancellationToken);
    }

    private static VoicemailTranscription MapPayloadToTranscription(WebhookPayloadDto payload, string transcription)
    {
        var message = payload.Body;

        var fromContact = new ContactInfo(message.From.PhoneNumber, message.From.Name);

        var to = message.To.First();
        var toContact = new ContactInfo(to.PhoneNumber, to.Name);

        var transcriptionAttachment = payload.Body.Attachments
            .FirstOrDefault(a => a.Type == "AudioTranscription");

        var audioAttachment = payload.Body.Attachments
            .FirstOrDefault(a => a.Type == "AudioRecording");

        return new VoicemailTranscription
        {
            RingCentralUuid = payload.Uuid,
            CallDateTime = message.CreationTime,
            From = fromContact,
            To = toContact,
            TranscriptionText = transcription,
            AudioTranscriptionUri = transcriptionAttachment?.Uri,
            AudioRecordingUri = audioAttachment?.Uri
        };
    }
}
