using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Infrastructure.Services;

public class TranscriptionWriter(IFtpFileService ftpFileService, ILogger<TranscriptionWriter> logger) : ITranscriptionWriter
{
    public async Task WriteTranscriptionToFtpAsync(ActivityContext<TranscriptionInput> context, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Payload {PayloadUuid} - Writing voicemail transcription to FTP file", context.PayloadUuid);

        var transcriptionEntry = GenerateTranscriptionEntry(context.Data.Payload, context.Data.Transcription);

        var fileExists = await ftpFileService.FileExistsAsync(context.PayloadUuid, cancellationToken);

        await ftpFileService.WriteToFileAsync(fileExists, transcriptionEntry, cancellationToken);
    }

    private static TranscriptionEntry GenerateTranscriptionEntry(WebhookPayloadDto payload, string transcription)
    {
        var message = payload.Body!;

        return new TranscriptionEntry(payload.Uuid,
            message.CreationTime,
            message.From.PhoneNumber,
            message.To.First().PhoneNumber,
            transcription
        );
    }
}
