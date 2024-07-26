namespace SpotifyAPI.Services.Interfaces;

public interface IExistingEntityCheckerService
{
    Task<bool> IsCorrectArtistId(long id, CancellationToken cancellationToken);
    Task<bool> IsCorrectAlbumId(long id, CancellationToken cancellationToken);
    Task<bool> IsCorrectTrackId(long id, CancellationToken cancellationToken);
    Task<bool> IsCorrectPlaylistId(long id, CancellationToken cancellationToken);
    Task<bool> IsCorrectGenreId(long id, CancellationToken cancellationToken);
    Task<bool> IsAvailableGenreName(string name, CancellationToken cancellationToken);
}