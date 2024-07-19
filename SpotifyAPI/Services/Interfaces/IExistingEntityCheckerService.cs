namespace SpotifyAPI.Services.Interfaces;

public interface IExistingEntityCheckerService
{
    Task<bool> IsCorrectArtistId(long id, CancellationToken cancellationToken);
    Task<bool> IsCorrectAlbumId(long id, CancellationToken cancellationToken);
}