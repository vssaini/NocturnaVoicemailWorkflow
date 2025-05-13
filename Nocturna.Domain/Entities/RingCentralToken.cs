namespace Nocturna.Domain.Entities;

public record RingCentralToken(
    int Id,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);
