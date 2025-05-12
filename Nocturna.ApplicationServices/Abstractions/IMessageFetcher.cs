using Nocturna.Domain.Models;
using Nocturna.Domain.Models.RingCentral;

namespace Nocturna.Application.Abstractions;

public interface IMessageFetcher
{
    Task<MessageDto> GetMessageAsync(MessageRequest request, CancellationToken cancellationToken = default);
}