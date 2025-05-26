using Nocturna.Domain.Enums;

namespace Nocturna.Domain.Models;

public record TranscriptionResult(long? AttachmentId, VmTranscriptionStatus TranscriptionStatus);