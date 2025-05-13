namespace Nocturna.Domain.Models;

public record RingCentralTokenDto(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt,
    DateTime RefreshTokenExpiresAt
);
