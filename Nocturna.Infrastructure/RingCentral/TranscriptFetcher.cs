using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Infrastructure.Policies;
using Nocturna.Infrastructure.RingCentral.Clients;

namespace Nocturna.Infrastructure.RingCentral;

public class TranscriptFetcher(IRingCentralMediaApi mediaApi, ILogger<TranscriptFetcher> logger) : ITranscriptFetcher
{
    public async Task<string> GetTranscriptionAsync(ActivityContext<TranscriptionRequest> context, CancellationToken cancellationToken = default)
    {
        var request = context.Data;
        logger.LogInformation("Payload {PayloadUuid} - Calling RingCentral Media API to fetch transcription from path 'account/{AccountId}/extension/{ExtensionId}/message-store/{MessageId}/content/{AttachmentId}'", context.PayloadUuid, request.AccountId, request.ExtensionId, request.MessageId, request.AttachmentId);

        var apiRetryPolicy = ApiPolicy.CreateHttpRetryPolicy(context.PayloadUuid, logger);

        return await apiRetryPolicy.ExecuteAsync(() =>
            mediaApi.GetMessageAttachmentContentAsync(
                request.AccountId,
                request.ExtensionId,
                request.MessageId,
                request.AttachmentId,
                request.ContentDisposition.ToString(),
                cancellationToken));
    }
}
