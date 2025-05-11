namespace Nocturna.Domain.Entities;

public class VoicemailTranscription
{
    public string RingCentralUuid { get; init; }

    public DateTime CallDateTime { get; init; }
    public ContactInfo From { get; init; }
    public ContactInfo To { get; init; }
    public string TranscriptionText { get; init; }

    public string AudioTranscriptionUri { get; init; }
    public string AudioRecordingUri { get; init; }
}
