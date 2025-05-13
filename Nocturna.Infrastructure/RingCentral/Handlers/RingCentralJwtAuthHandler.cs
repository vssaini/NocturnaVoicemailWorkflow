using Nocturna.Application.Abstractions;
using System.Net.Http.Headers;

namespace Nocturna.Infrastructure.RingCentral.Handlers;

public class RingCentralJwtAuthHandler(ITokenService tokenService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenService.GetValidAccessTokenAsync();

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, cancellationToken);
    }
}