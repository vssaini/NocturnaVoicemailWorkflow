using RingCentral;

namespace Nocturna.Application.Abstractions;

public interface IRingCentralClientFactory
{
    RestClient CreateClient();
}