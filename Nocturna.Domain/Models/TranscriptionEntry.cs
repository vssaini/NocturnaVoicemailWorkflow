namespace Nocturna.Domain.Models;

public record TranscriptionEntry
{
    public string Uuid { get; init; }
    public DateTime CreationTime { get; init; }
    public string FromPhoneNumber { get; init; }
    public string ToPhoneNumber { get; init; }
    public string Transcription { get; init; }
}