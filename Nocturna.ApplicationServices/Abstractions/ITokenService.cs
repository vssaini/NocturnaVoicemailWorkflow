namespace Nocturna.Application.Abstractions;

public interface ITokenService
{
    Task<string> GetValidAccessTokenAsync();
}