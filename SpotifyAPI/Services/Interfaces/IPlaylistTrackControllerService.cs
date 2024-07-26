using SpotifyAPI.Models.PlaylistTrack;

namespace SpotifyAPI.Services.Interfaces;

public interface IPlaylistTrackControllerService
{
    Task CreateAsync(PlaylistTrackCreateVm vm);
    Task DeleteIfExistsAsync(PlaylistTrackDeleteVm vm);
}