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
    /// <param name="request">The transcription request containing message and attachment identifiers.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The transcription text as a string.</returns>
    Task<string> GetTranscriptionAsync(TranscriptionRequest request, CancellationToken cancellationToken = default);
}