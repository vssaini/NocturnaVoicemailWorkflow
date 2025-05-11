using Microsoft.Azure.Functions.Worker.Http;

namespace Nocturna.Application.Abstractions;

public interface IVoicemailService
{
    Task<HttpResponseData> ProcessVoicemailAsync(HttpRequestData req, string payload, CancellationToken cancellationToken = default);
}