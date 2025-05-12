using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Abstractions;
using Nocturna.Domain.Entities;
using Nocturna.Domain.Enums;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Infrastructure.Policies;
using Polly.Retry;

namespace Nocturna.Infrastructure.Services;

public class VoicemailProcessor(
    ITranscriptFetcher transcriptFetcher,
    IVoicemailRepository voicemailRepository,
    ILogger<VoicemailProcessor> logger)
    : IVoicemailProcessor
{
    private readonly AsyncRetryPolicy _dbRetryPolicy = DbPollyPolicy.CreateDefaultRetryPolicy(logger);

    public async Task<int> SavePayloadAsync(string payload, CancellationToken cancellationToken = default)
    {
        var payloadId = await _dbRetryPolicy.ExecuteAsync(async ct =>
            await voicemailRepository.SaveWebhookPayloadAsync(payload, ct), cancellationToken);

        return payloadId;
    }

    public async Task<string> GetTranscriptionAsync(VoicemailMessage voicemailMessage, CancellationToken cancellationToken = default)
    {
        var request = new TranscriptionRequest(voicemailMessage.MessageId, voicemailMessage.AttachmentId, ContentDisposition.Inline);
        var transcription = await transcriptFetcher.GetTranscriptionAsync(request, cancellationToken);

        if (string.IsNullOrWhiteSpace(transcription))
        {
            logger.LogWarning("Transcription for message {MessageId} (attachment {AttachmentId}) is empty or null.", voicemailMessage.MessageId, voicemailMessage.AttachmentId);
            return string.Empty;
        }

        logger.LogInformation("Audio transcription for message {MessageId} (attachment {AttachmentId}): {Transcription}", voicemailMessage.MessageId, voicemailMessage.AttachmentId, transcription);
        return transcription;
    }

    public async Task SaveVoicemailTranscriptionAsync(WebhookPayloadDto payload, string transcription, int dbPayloadId, CancellationToken cancellationToken = default)
    {
        var vmTranscription = MapPayloadToTranscription(payload, transcription);

        await _dbRetryPolicy.ExecuteAsync(async ct =>
            await voicemailRepository.SaveVoicemailTranscriptionAsync(dbPayloadId, vmTranscription, ct), cancellationToken);
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
