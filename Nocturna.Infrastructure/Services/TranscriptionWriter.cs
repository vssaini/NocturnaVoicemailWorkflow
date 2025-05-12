using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Infrastructure.Services;

public class TranscriptionWriter(IFtpFileService ftpFileService, ILogger<TranscriptionWriter> logger) : ITranscriptionWriter
{
    public async Task WriteTranscriptionToFtpAsync(WebhookPayloadDto payload, string transcription, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Writing voicemail transcription payload {PayloadUuId} to FTP file", payload.Uuid);

        var transcriptionEntry = GenerateTranscriptionEntry(payload, transcription);

        var fileExists = await ftpFileService.FileExistsAsync(cancellationToken);

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
