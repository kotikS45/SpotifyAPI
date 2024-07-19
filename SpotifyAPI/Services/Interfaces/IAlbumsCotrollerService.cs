using SpotifyAPI.Models.Album;

namespace SpotifyAPI.Services.Interfaces;

public interface IAlbumsCotrollerService
{
    Task CreateAsync(AlbumCreateVm vm);
}