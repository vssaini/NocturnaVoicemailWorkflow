using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Infrastructure.Helpers;

namespace Nocturna.Infrastructure.Services;

public class VoicemailService(
    ILogger<VoicemailService> logger,
    IVoicemailProcessor processor,
    IVoicemailWebhookParser parser,
    ITranscriptionWriter transWriter
    ) : IVoicemailService
{
    public async Task<HttpResponseData> ProcessVoicemailAsync(HttpRequestData req, string payload, CancellationToken cancellationToken = default)
    {
        var dbPayloadId = await processor.SavePayloadAsync(payload, cancellationToken);

        var payloadDto = parser.ParsePayload(payload);
        if (!parser.IsValidPayload(payloadDto))
        {
            logger.LogWarning("Invalid payload: {@Payload}", payloadDto);
            return await VoicemailResponder.CreateErrorAsync(req, "Invalid payload", cancellationToken);
        }

        if (!parser.IsVoicemailEvent(payloadDto!))
        {
            logger.LogWarning("Unsupported event type: {Event}. Only 'voicemail' events are supported.", payloadDto!.Event);
            return await VoicemailResponder.CreateErrorAsync(req, "Invalid event type", cancellationToken);
        }

        var message = payloadDto!.Body;
        var attachmentId= parser.GetTranscriptionAttachmentId(message);
        if (attachmentId is null)
        {
            logger.LogWarning("Audio transcription skipped: No attachment ID found.");
            return await VoicemailResponder.CreateErrorAsync(req, "No attachment id found", cancellationToken);
        }

        var voicemailMsg = new VoicemailMessage(payloadDto.Body.Id, attachmentId.Value);
        var transcription = await processor.GetTranscriptionAsync(voicemailMsg, cancellationToken);
        if (string.IsNullOrWhiteSpace(transcription))
        {
            logger.LogWarning("Transcription is empty or null for message {MessageId} (attachment {AttachmentId}).", message.Id, attachmentId);
            return await VoicemailResponder.CreateErrorAsync(req, "Failed to fetch transcription", cancellationToken);
        }

        await processor.SaveVoicemailTranscriptionAsync(payloadDto, transcription, dbPayloadId, cancellationToken);
        await transWriter.WriteTranscriptionToFtpAsync(payloadDto, transcription, cancellationToken);

        return await VoicemailResponder.CreateSuccessAsync(req, message.Id, transcription, cancellationToken);
    }
}