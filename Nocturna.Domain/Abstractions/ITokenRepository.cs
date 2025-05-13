using Nocturna.Domain.Entities;
using Nocturna.Domain.Models;

namespace Nocturna.Domain.Abstractions;

public interface ITokenRepository
{
    Task<RingCentralToken?> GetTokenAsync(CancellationToken cancellationToken = default);
    Task SaveTokenAsync(RingCentralTokenDto token, CancellationToken cancellationToken = default);
    Task UpdateTokenAsync(int id, RingCentralTokenDto token, CancellationToken cancellationToken = default);
}