using SpotifyAPI.Models.Follower;

namespace SpotifyAPI.Services.Interfaces;

public interface IFollowerControllerService
{
    Task Follow(long userId, long artistId);
    Task Unfollow(long userId, long artistId);
}