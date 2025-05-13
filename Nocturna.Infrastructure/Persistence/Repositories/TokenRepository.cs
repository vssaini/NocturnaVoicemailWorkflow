using Dapper;
using Microsoft.Extensions.Logging;
using Nocturna.Domain.Abstractions;
using Nocturna.Domain.Entities;
using Nocturna.Domain.Models;

namespace Nocturna.Infrastructure.Persistence.Repositories;

public class TokenRepository(IDbConnectionFactory dbConnectionFactory, ILogger<TokenRepository> logger) : ITokenRepository
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
        FROM dbo.RingCentralTokens
        ORDER BY CreatedAtUtc DESC;";

        using var connection = await dbConnectionFactory.CreateConnectionAsync(cancellationToken);
        var command = new CommandDefinition(sql, cancellationToken: cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<RingCentralToken>(command);
    }

    public async Task SaveTokenAsync(RingCentralTokenDto token, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating new RingCentral token.");
        await SaveOrUpdateTokenAsync(null, token, cancellationToken, "dbo.usp_CreateRingCentralToken");
    }

    public async Task UpdateTokenAsync(int id, RingCentralTokenDto token, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating RingCentral token with Id {TokenId}.", id);
        await SaveOrUpdateTokenAsync(id, token, cancellationToken, "dbo.usp_UpdateRingCentralToken");
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
