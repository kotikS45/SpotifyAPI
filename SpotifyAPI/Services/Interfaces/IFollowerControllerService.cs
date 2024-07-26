using SpotifyAPI.Models.Follower;

namespace SpotifyAPI.Services.Interfaces;

public interface IFollowerControllerService
{
    Task Follow(long userId, FollowerVm vm);
    Task Unfollow(long userId, FollowerVm vm);
}