using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
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
        var dbPayloadId = await processor.SaveWebhookPayloadAsync(payload, req.Url.ToString(), cancellationToken);

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
        var transcriptionId = parser.GetTranscriptionAttachmentId(message);
        if (transcriptionId is null)
        {
            logger.LogWarning("Audio transcription skipped: No attachment ID found.");
            return await VoicemailResponder.CreateErrorAsync(req, "No attachment id found", cancellationToken);
        }

        var transcription = await processor.GetTranscriptionAsync(message.Id, transcriptionId.Value, cancellationToken);
        if (string.IsNullOrWhiteSpace(transcription))
        {
            logger.LogWarning("Transcription is empty or null for message {MessageId} (attachment {AttachmentId}).", message.Id, transcriptionId);
            return await VoicemailResponder.CreateErrorAsync(req, "Failed to fetch transcription", cancellationToken);
        }

        await processor.SaveVoicemailTranscriptionAsync(payloadDto, transcription, dbPayloadId, cancellationToken);
        await transWriter.WriteTranscriptionToFtpAsync(payloadDto, transcription, cancellationToken);

        return await VoicemailResponder.CreateSuccessAsync(req, message.Id, transcription, cancellationToken);
    }
}