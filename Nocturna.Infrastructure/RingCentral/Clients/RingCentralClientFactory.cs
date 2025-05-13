using Microsoft.Extensions.Options;
using Nocturna.Application.Abstractions;
using Nocturna.Domain.Config;
using RingCentral;

namespace Nocturna.Infrastructure.RingCentral.Clients;

public class RingCentralClientFactory(IOptions<RingCentralSettings> options) : IRingCentralClientFactory
{
    private readonly RingCentralSettings _settings = options.Value;

    public RestClient CreateClient()
    {
        return new RestClient(_settings.ClientId, _settings.ClientSecret, _settings.ServerUrl);
    }
}