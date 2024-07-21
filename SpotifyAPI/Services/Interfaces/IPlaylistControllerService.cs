using SpotifyAPI.Models.Playlist;

namespace SpotifyAPI.Services.Interfaces;

public interface IPlaylistControllerService
{
    Task CreateAsync(PlaylistCreateVm vm, long userId);
    Task UpdateAsync(PlaylistUpdateVm vm);
    Task DeleteIfExistsAsync(long id);
}