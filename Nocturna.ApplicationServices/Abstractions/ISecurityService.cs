using Microsoft.Azure.Functions.Worker.Http;
using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

/// <summary>
/// Service that performs security validation on incoming webhook requests from RingCentral
/// using a developer-defined validation token set during subscription creation (deliveryMode.validationToken).
/// </summary>
public interface ISecurityService
{
    /// <summary>
    /// Verifies the 'validation-token' header on incoming webhook requests.
    /// 
    /// This token is defined by the developer during webhook subscription setup and is sent by RingCentral
    /// with every subsequent webhook request. This method compares the token in the request header
    /// against the expected value stored in configuration (e.g., app settings or Key Vault).
    /// 
    /// - If the token is missing or doesn't match, it returns a 401 Unauthorized response.
    /// - If the token is valid, it returns a 200 OK response and flags the request as valid.
    /// 
    /// This validation protects against spoofed or malicious webhook requests.
    /// </summary>
    /// <param name="req">The incoming HTTP request from RingCentral.</param>
    /// <param name="cancellationToken">Cancellation token for async operations.</param>
    /// <returns>
    /// A <see cref="SecurityVerificationResult"/> indicating whether the token was valid,
    /// and containing an appropriate HTTP response to be returned from the Azure Function.
    /// </returns>
    Task<SecurityVerificationResult> VerifyTokenAsync(HttpRequestData req, CancellationToken cancellationToken = default);
}