using Microsoft.Extensions.Options;
using RingCentral;
using System.Net.Http.Headers;
using Nocturna.Domain.Config;

namespace Nocturna.Infrastructure.RingCentral.Handlers;

public class RingCentralJwtAuthHandler : DelegatingHandler
{
    private readonly RingCentralSettings _rcSettings;
    private readonly RestClient _client;
    private string? _cachedToken;

    public RingCentralJwtAuthHandler(IOptions<RingCentralSettings> options)
    {
        _rcSettings = options.Value;
        _client = GetClient();
    }

    private RestClient GetClient()
    {
        return new RestClient(_rcSettings.ClientId, _rcSettings.ClientSecret, _rcSettings.ServerUrl);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_cachedToken))
            await AuthenticateAsync();

        var response = await SendWithTokenAsync(request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await RefreshTokenAsync();
            response = await SendWithTokenAsync(request, cancellationToken);
        }

        return response;
    }

    private async Task<HttpResponseMessage> SendWithTokenAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _cachedToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task AuthenticateAsync()
    {
        // TODO: Brainstorm if we should save the token in the database for better performance
        // And what is the expiry time.
        await _client.Authorize(_rcSettings.JwtToken);
        _cachedToken = _client.token.access_token;
    }

    private async Task RefreshTokenAsync()
    {
        // TODO: Check if we can refresh token or not
        // RingCentral JWT tokens are single-use; re-authenticate
        await AuthenticateAsync();
    }
}