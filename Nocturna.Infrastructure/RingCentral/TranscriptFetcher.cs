using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Infrastructure.Policies;
using Nocturna.Infrastructure.RingCentral.Clients;
using Polly.Retry;

namespace Nocturna.Infrastructure.RingCentral;

public class TranscriptFetcher(IRingCentralMediaApi mediaApi, ILogger<TranscriptFetcher> logger) : ITranscriptFetcher
{
    private readonly AsyncRetryPolicy _apiRetryPolicy = RingCentralApiPolicy.CreateHttpRetryPolicy(logger);

    public async Task<string> GetTranscriptionAsync(TranscriptionRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Calling RingCentral Media API to fetch transcription for message {MessageId} (attachment {AttachmentId})", request.MessageId, request.AttachmentId);

        return await _apiRetryPolicy.ExecuteAsync(() =>
            mediaApi.GetMessageAttachmentContentAsync(
                request.MessageId,
                request.AttachmentId,
                request.ContentDisposition.ToString(),
                cancellationToken));
    }
}
