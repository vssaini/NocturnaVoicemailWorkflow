using Nocturna.Domain.Enums;

namespace Nocturna.Domain.Models;

public record TranscriptionRequest(long AccountId, long ExtensionId, long MessageId, long AttachmentId, ContentDisposition ContentDisposition);