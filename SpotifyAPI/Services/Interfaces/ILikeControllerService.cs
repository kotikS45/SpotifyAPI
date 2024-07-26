using SpotifyAPI.Models.Like;

namespace SpotifyAPI.Services.Interfaces;

public interface ILikeControllerService
{
    Task Like(long userId, LikeVm vm);
    Task Unlike(long userId, LikeVm vm);
}