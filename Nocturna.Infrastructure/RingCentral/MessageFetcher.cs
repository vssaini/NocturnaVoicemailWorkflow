using Microsoft.Extensions.Logging;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;
using Nocturna.Infrastructure.Policies;
using Nocturna.Infrastructure.RingCentral.Clients;

namespace Nocturna.Infrastructure.RingCentral;

public class MessageFetcher(IRingCentralApi rcApi, ILogger<MessageFetcher> logger) : IMessageFetcher
{
    public async Task<MessageDto> GetMessageAsync(ActivityContext<MessageRequest> context, CancellationToken cancellationToken = default)
    {
        var msg = context.Data;
        logger.LogInformation("Payload {PayloadUuid} - Fetching message {MessageId} from path 'account/{AccountId}/extension/{ExtensionId}/message-store/{MessageId}'", context.PayloadUuid, msg.MessageId, msg.AccountId, msg.ExtensionId, msg.MessageId);

        var apiRetryPolicy = ApiPolicy.CreateHttpRetryPolicy(context.PayloadUuid, logger);

        return await apiRetryPolicy.ExecuteAsync((ct) =>
        rcApi.GetMessageAsync(
            msg.AccountId,
            msg.ExtensionId,
            msg.MessageId,
            ct), cancellationToken);
    }
}
