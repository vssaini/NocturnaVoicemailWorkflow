using Microsoft.Azure.Functions.Worker.Http;
using System.Net;

namespace Nocturna.Infrastructure.Helpers;

public static class VoicemailResponder
{
    public static async Task<HttpResponseData> CreateErrorAsync(HttpRequestData req, string message, CancellationToken cancellationToken = default)
    {
        var response = req.CreateResponse(HttpStatusCode.BadRequest);
        await response.WriteAsJsonAsync(new { status = "error", message }, cancellationToken: cancellationToken);
        return response;
    }

    public static async Task<HttpResponseData> CreateSuccessAsync(HttpRequestData req, long payloadBodyId, string transcription,
        CancellationToken cancellationToken = default)
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new
        {
            status = "success",
            message = "Voicemail event received.",
            id = payloadBodyId,
            transcription
        }, cancellationToken: cancellationToken);

        return response;
    }
}