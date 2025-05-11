using Nocturna.Domain.Enums;

namespace Nocturna.Domain.Models;

public record RequestResult
{
    public RequestStatus Status { get; init; }
    public string Message { get; init; }
    public string? Transcription { get; init; }
    public bool IsValidationRequest { get; init; }
    public string? ValidationToken { get; init; }

    /// <summary>
    /// Private constructor.
    /// </summary>
    private RequestResult(
        RequestStatus status,
        string message,
        string? transcription,
        bool isValidationRequest,
        string? validationToken)
    {
        Status = status;
        Message = message;
        Transcription = transcription;
        IsValidationRequest = isValidationRequest;
        ValidationToken = validationToken;
    }

    /// <summary>
    /// Create a new instance of <see cref="RequestResult"/>.
    /// </summary>
    public static RequestResult Create(
        RequestStatus status,
        string message,
        string? transcription = null,
        bool isValidationRequest = false,
        string? validationToken = null)
    {
        return new RequestResult(status, message, transcription, isValidationRequest, validationToken);
    }
}
