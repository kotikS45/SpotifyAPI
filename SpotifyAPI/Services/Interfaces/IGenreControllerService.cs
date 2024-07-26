using SpotifyAPI.Models.Genre;

namespace SpotifyAPI.Services.Interfaces;

public interface IGenreControllerService
{
    Task CreateAsync(GenreCreateVm vm);
    Task UpdateAsync(GenreUpdateVm vm);
    Task DeleteIfExistsAsync(long id);
}
