using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

/// <summary>
/// Fetches voicemail transcription content from the RingCentral Media API.
/// </summary>
public interface ITranscriptFetcher
{
    /// <summary>
    /// Retrieves the transcription text for a given voicemail message and attachment.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The transcription text as a string.</returns>
    Task<string> GetTranscriptionAsync(ActivityContext<TranscriptionRequest> context, CancellationToken cancellationToken = default);
}