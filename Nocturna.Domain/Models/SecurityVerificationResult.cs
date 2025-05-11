using Microsoft.Azure.Functions.Worker.Http;

namespace Nocturna.Domain.Models;

public class SecurityVerificationResult
{
    public bool IsValid { get; set; }
    public HttpResponseData Response { get; set; }
}