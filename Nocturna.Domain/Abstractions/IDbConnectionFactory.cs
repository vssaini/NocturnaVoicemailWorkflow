using System.Data;

namespace Nocturna.Domain.Abstractions;

public interface IDbConnectionFactory
{
    Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default);
}
