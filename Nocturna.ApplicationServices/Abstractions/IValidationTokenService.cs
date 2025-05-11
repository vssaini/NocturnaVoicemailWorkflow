using Microsoft.Azure.Functions.Worker.Http;

namespace Nocturna.Application.Abstractions;

/// <summary>
/// Service responsible for handling webhook validation from RingCentral during subscription setup.
/// </summary>
public interface IValidationTokenService
{
    /// <summary>
    /// Handles webhook validation requests received from RingCentral during subscription setup.
    /// 
    /// This method checks if the incoming request contains a 'Validation-Token' header.
    /// If present, this indicates that RingCentral is attempting to verify that the webhook endpoint is accessible.
    /// 
    /// In such a case, the method creates an HTTP 200 OK response and echoes the token back
    /// in the response headers, as required by RingCentral's webhook verification process.
    /// 
    /// If the header is not present, it means this is a regular webhook event notification, 
    /// and the method returns null to allow normal processing to continue.
    /// </summary>
    /// <param name="req">The HTTP request received from RingCentral.</param>
    /// <param name="payload">The payload from the HTTP request.</param>
    /// <returns>
    /// An HttpResponseData containing the echoed Validation-Token if this is a setup verification request,
    /// or null if the request should continue through the normal webhook event pipeline.
    /// </returns>
    HttpResponseData HandleValidationToken(HttpRequestData req, string payload);
}