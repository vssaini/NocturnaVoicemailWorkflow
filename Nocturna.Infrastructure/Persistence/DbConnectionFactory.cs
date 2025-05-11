using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Nocturna.Domain.Abstractions;
using System.Data;

namespace Nocturna.Infrastructure.Persistence;

public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")
                                                ?? throw new ArgumentNullException(nameof(_connectionString), "Database connection string is missing.");

    public async Task<IDbConnection> CreateConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}