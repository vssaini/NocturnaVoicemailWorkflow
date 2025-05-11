using Nocturna.Domain.Enums;

namespace Nocturna.Domain.Models;

public record TranscriptionRequest(long MessageId, long AttachmentId, ContentDisposition ContentDisposition);