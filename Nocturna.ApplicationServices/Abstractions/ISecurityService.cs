using Microsoft.Azure.Functions.Worker.Http;
using Nocturna.Domain.Models;

namespace Nocturna.Application.Abstractions;

/// <summary>
/// Service that performs security verification on incoming webhook requests from RingCentral
/// using a developer-defined verification-token set during subscription creation (deliveryMode.validationToken).
/// </summary>
public interface ISecurityService
{
    /// <summary>
    /// Verifies the 'verification-token' header on incoming webhook requests.
    /// <para>
    /// This token is defined by the developer during webhook subscription setup and is sent by RingCentral
    /// with every subsequent webhook request. This method compares the token in the request header
    /// against the expected value stored in configuration (e.g., app settings or Key Vault).
    /// </para>
    /// <para>
    /// This verification protects against spoofed or malicious webhook requests.
    /// </para>
    /// </summary>
    /// <param name="req">The incoming HTTP request from RingCentral.</param>
    /// <returns>
    /// A <see cref="SecurityVerificationResult"/> indicating whether the token was verified,
    /// and containing an appropriate HTTP response to be returned from the Azure Function.
    /// </returns>
    SecurityVerificationResult VerifyToken(HttpRequestData req);
}