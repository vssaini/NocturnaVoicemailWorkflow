using Nocturna.Domain.Enums;

namespace Nocturna.Domain.Models;

public record RequestResult
{
    public RequestStatus Status { get; init; }
    public string Message { get; init; }
    public string? Transcription { get; init; }

    /// <summary>
    /// Private constructor.
    /// </summary>
    private RequestResult(RequestStatus status, string message, string? transcription)
    {
        Status = status;
        Message = message;
        Transcription = transcription;
    }

    /// <summary>
    /// Create a new instance of <see cref="RequestResult"/>.
    /// </summary>
    public static RequestResult Create(RequestStatus status, string message, string? transcription = null)
    {
        return new RequestResult(status, message, transcription);
    }
}
