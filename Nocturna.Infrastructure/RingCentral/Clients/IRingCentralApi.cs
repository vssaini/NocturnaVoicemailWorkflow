using Nocturna.Domain.Models.RingCentral;
using Refit;

namespace Nocturna.Infrastructure.RingCentral.Clients;

/// <summary>
/// Interface for interacting with RingCentral's API for retrieving messages.
/// </summary>
public interface IRingCentralApi
{
    /// <summary>
    /// Gets a message by its ID from the specified account and extension.
    /// </summary>
    /// <param name="accountId">The ID of the RingCentral account.</param>
    /// <param name="extensionId">The ID of the extension within the account.</param>
    /// <param name="messageId">The ID of the message to retrieve.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests. Default is <c>default</c>.</param>
    /// <returns>A task representing the asynchronous operation, with a <see cref="MessageDto"/> as the result.</returns>
    [Get("/restapi/v1.0/account/{accountId}/extension/{extensionId}/message-store/{messageId}")]
    Task<MessageDto> GetMessageAsync(
        long accountId,
        long extensionId,
        long messageId,
        CancellationToken cancellationToken = default);
}

