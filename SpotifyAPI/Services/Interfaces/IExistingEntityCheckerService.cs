namespace SpotifyAPI.Services.Interfaces;

public interface IExistingEntityCheckerService
{
    Task<bool> IsCorrectArtistId(long id, CancellationToken cancellationToken);
}