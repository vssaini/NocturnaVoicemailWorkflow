using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Abstractions;
using Nocturna.Domain.Config;
using Nocturna.Domain.Entities;
using Nocturna.Domain.Models;
using Nocturna.Infrastructure.Policies;
using Polly.Retry;
using RingCentral;

namespace Nocturna.Infrastructure.Services;

public class TokenService(IOptions<RingCentralSettings> options,
    IRingCentralClientFactory clientFactory,
    ITokenRepository tokenRepository, 
    ILogger<TokenService> logger) : ITokenService
{
    private readonly RingCentralSettings _rcSettings = options.Value;

    private readonly AsyncRetryPolicy _apiRetryPolicy = RingCentralApiPolicy.CreateHttpRetryPolicy(logger);

    // Ref - https://developers.ringcentral.com/guide/authentication/refresh-tokens
    // RingCentral access tokens last 1 hour; refresh tokens are valid for 7 days.

    public async Task<string> GetValidAccessTokenAsync()
    {
        var storedToken = await tokenRepository.GetTokenAsync();
        if (storedToken == null)
        {
            logger.LogInformation("No token found, requesting new token using JWT.");
            return await AuthorizeWithJwtAsync();
        }

        if (IsAccessTokenExpired(storedToken))
        {
            logger.LogWarning("Access token expired, attempting to refresh.");
            return await RefreshTokenAsync(storedToken);
        }

        return storedToken.AccessToken;
    }

    private static bool IsAccessTokenExpired(RingCentralToken token)
        => token.AccessTokenExpiresAt <= DateTime.UtcNow;

    private async Task<string> AuthorizeWithJwtAsync()
    {
        var client = clientFactory.CreateClient();
        await _apiRetryPolicy.ExecuteAsync(() =>
            client.Authorize(_rcSettings.JwtToken)
            );

        if (client.token == null)
            throw new InvalidOperationException("Token was not returned from RingCentral after JWT authorization.");

        await SaveOrUpdateTokenAsync(client);
        return client.token.access_token;
    }

    private async Task SaveOrUpdateTokenAsync(RestClient client)
    {
        var rcToken = MapToRingCentralTokenDto(client);

        var existingToken = await tokenRepository.GetTokenAsync();

        if (existingToken == null)
            await tokenRepository.SaveTokenAsync(rcToken);
        else
            await tokenRepository.UpdateTokenAsync(existingToken.Id, rcToken);
    }

    private static RingCentralTokenDto MapToRingCentralTokenDto(RestClient client)
    {
        return new RingCentralTokenDto(
            client.token.access_token,
            client.token.refresh_token,
            DateTime.UtcNow.AddSeconds((double)client.token.expires_in!),
            DateTime.UtcNow.AddSeconds((double)client.token.refresh_token_expires_in!)
        );
    }

    private async Task<string> RefreshTokenAsync(RingCentralToken storedToken)
    {
        if (IsRefreshTokenExpired(storedToken))
        {
            logger.LogWarning("Refresh token expired, re-authorizing using JWT.");
            return await AuthorizeWithJwtAsync();
        }

        // Refreshing a token provides a new access token and refresh token, both with updated expiration times.
        var client = clientFactory.CreateClient();
        await _apiRetryPolicy.ExecuteAsync(() =>
            client.Refresh(storedToken.RefreshToken));

        if (client.token == null)
            throw new InvalidOperationException("Token was not returned from RingCentral after refresh.");

        await SaveOrUpdateTokenAsync(client);
        return client.token.access_token;
    }

    private static bool IsRefreshTokenExpired(RingCentralToken token)
        => token.RefreshTokenExpiresAt <= DateTime.UtcNow;
}
