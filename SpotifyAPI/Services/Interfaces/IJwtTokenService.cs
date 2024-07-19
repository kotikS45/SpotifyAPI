using Model.Entities.Identity;

namespace SpotifyAPI.Services.Interfaces;

public interface IJwtTokenService
{
    Task<string> CreateTokenAsync(User user);
}