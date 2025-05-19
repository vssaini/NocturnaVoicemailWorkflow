namespace Nocturna.Domain.Models;

public record VoicemailMessage(long AccountId, long ExtensionId, long MessageId, long AttachmentId);