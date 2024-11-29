using SpotifyAPI.Models.Like;

namespace SpotifyAPI.Services.Interfaces;

public interface ILikeControllerService
{
    Task Like(long userId, long id);
    Task Unlike(long userId, long id);
}