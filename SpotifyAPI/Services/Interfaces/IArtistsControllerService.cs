using SpotifyAPI.Models.Artist;

namespace SpotifyAPI.Services.Interfaces;

public interface IArtistsControllerService
{
    Task CreateAsync(ArtistCreateVm vm);
    Task UpdateAsync(ArtistUpdateVm vm);
    Task DeleteIfExistsAsync(long id);
}