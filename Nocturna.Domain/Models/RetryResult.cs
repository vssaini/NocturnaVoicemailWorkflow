namespace Nocturna.Domain.Models;

public record RetryResult(long? AttachmentId, bool ShouldRetry);