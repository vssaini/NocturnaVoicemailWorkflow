using Microsoft.Azure.Functions.Worker.Http;

namespace Nocturna.Application.Abstractions;

public interface IWebhookUseCase
{
    Task<HttpResponseData> ExecuteAsync(HttpRequestData req, string payload, CancellationToken cancellationToken = default);
}