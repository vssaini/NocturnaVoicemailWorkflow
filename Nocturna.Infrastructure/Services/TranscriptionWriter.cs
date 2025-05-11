using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;

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

        return new TranscriptionEntry
        {
            Uuid = payload.Uuid,
            CreationTime = message.CreationTime,
            FromPhoneNumber = message.From.PhoneNumber,
            ToPhoneNumber = message.To.First().PhoneNumber,
            Transcription = transcription
        };
    }
}
