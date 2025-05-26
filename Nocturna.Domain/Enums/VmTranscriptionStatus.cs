namespace Nocturna.Domain.Enums;

/// <summary>
/// Represents the status of a voicemail transcription in RingCentral.
/// </summary>
public enum VmTranscriptionStatus
{
    /// <summary>
    /// Transcription is not available for this voicemail.
    /// </summary>
    NotAvailable,

    /// <summary>
    /// Transcription is currently in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Transcription process timed out before completion.
    /// </summary>
    TimedOut,

    /// <summary>
    /// Transcription has been successfully completed.
    /// </summary>
    Completed,

    /// <summary>
    /// Transcription was completed, but some parts may be missing or incomplete.
    /// </summary>
    CompletedPartially,

    /// <summary>
    /// Transcription attempt failed due to an error.
    /// </summary>
    Failed,

    /// <summary>
    /// Transcription status is unknown.
    /// </summary>
    Unknown
}
