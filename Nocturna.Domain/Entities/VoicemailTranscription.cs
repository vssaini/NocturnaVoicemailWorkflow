namespace Nocturna.Domain.Entities;

public class VoicemailTranscription
{
    public required string RingCentralUuid { get; init; }

    public DateTime CallDateTime { get; init; }
    public required ContactInfo From { get; init; }
    public required ContactInfo To { get; init; }
    public required string TranscriptionText { get; init; }

    public string? AudioTranscriptionUri { get; init; }
    public string? AudioRecordingUri { get; init; }
}
