namespace Nocturna.Domain.Models;

public record ActivityContext<T>(string PayloadUuid, T Data);
