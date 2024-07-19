using SpotifyAPI.Models.Album;

namespace SpotifyAPI.Services.Interfaces;

public interface IAlbumsCotrollerService
{
    Task CreateAsync(AlbumCreateVm vm);
    Task UpdateAsync(AlbumUpdateVm vm);
    Task DeleteIfExistsAsync(long id);
}