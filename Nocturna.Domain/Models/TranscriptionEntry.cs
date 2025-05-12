namespace Nocturna.Domain.Models;

public record TranscriptionEntry(
    string Uuid,
    DateTime CreationTime,
    string FromPhoneNumber,
    string ToPhoneNumber,
    string Transcription);
