using Dapper;
using Nocturna.Domain.Abstractions;
using Nocturna.Domain.Entities;
using Nocturna.Domain.Models;

namespace Nocturna.Infrastructure.Persistence.Repositories;

public class TokenRepository(IDbConnectionFactory dbConnectionFactory) : ITokenRepository
{
    public async Task<RingCentralToken?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        const string sql = @"
        SELECT TOP 1
            Id,
            AccessToken,
            RefreshToken,
            AccessTokenExpiresAt,
            RefreshTokenExpiresAt,
            CreatedAtUtc,
            UpdatedAtUtc
        FROM comm.RingCentralTokens
        ORDER BY CreatedAtUtc DESC;";

        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<RingCentralToken>(command);
    }

    public async Task SaveTokenAsync(RingCentralTokenDto token, CancellationToken cancellationToken = default)
    {
        await SaveOrUpdateTokenAsync(null, token, cancellationToken, "comm.usp_CreateRingCentralToken");
    }

    public async Task UpdateTokenAsync(int id, RingCentralTokenDto token, CancellationToken cancellationToken = default)
    {
        await SaveOrUpdateTokenAsync(id, token, cancellationToken, "comm.usp_UpdateRingCentralToken");
    }

    private async Task SaveOrUpdateTokenAsync(int? id, RingCentralTokenDto token, CancellationToken cancellationToken, string storedProcedure)
    {
        var dParams = new DynamicParameters();

        if (id.HasValue)
            dParams.Add("@Id", id.Value);

        dParams.Add("@AccessToken", token.AccessToken);
        dParams.Add("@RefreshToken", token.RefreshToken);
        dParams.Add("@AccessTokenExpiresAt", token.AccessTokenExpiresAt);
        dParams.Add("@RefreshTokenExpiresAt", token.RefreshTokenExpiresAt);

        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var command = new CommandDefinition(storedProcedure, dParams, cancellationToken: cancellationToken);
        await connection.ExecuteAsync(command);
    }
}
