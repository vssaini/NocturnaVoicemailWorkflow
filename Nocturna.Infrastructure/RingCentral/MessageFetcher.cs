using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Infrastructure.Policies;
using Nocturna.Infrastructure.RingCentral.Clients;
using Polly.Retry;

namespace Nocturna.Infrastructure.RingCentral;

public class MessageFetcher(IRingCentralApi rcApi, ILogger<MessageFetcher> logger) : IMessageFetcher
{
    private readonly AsyncRetryPolicy _apiRetryPolicy = RingCentralApiPolicy.CreateHttpRetryPolicy(logger);

    public async Task<MessageDto> GetMessageAsync(MessageRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Calling RingCentral API to fetch message for message id {MessageId}", request.MessageId);

        return await _apiRetryPolicy.ExecuteAsync(() =>
            rcApi.GetMessageAsync(
                request.MessageId,
                cancellationToken));
    }
}
