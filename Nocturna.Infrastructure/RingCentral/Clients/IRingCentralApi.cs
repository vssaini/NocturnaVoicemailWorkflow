using Nocturna.Domain.Models.RingCentral;
using Refit;

namespace Nocturna.Infrastructure.RingCentral.Clients;

public interface IRingCentralApi
{
    [Get("/restapi/v1.0/account/~/extension/~/message-store/{messageId}")]
    Task<MessageDto> GetMessageAsync(
        long messageId,
        CancellationToken cancellationToken = default);
}