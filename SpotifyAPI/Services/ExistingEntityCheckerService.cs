using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class ExistingEntityCheckerService(
    DataContext context
    ) : IExistingEntityCheckerService
{
    public async Task<bool> IsCorrectArtistId(long id, CancellationToken cancellationToken) => 
        await context.Artists.AnyAsync(a => a.Id == id, cancellationToken);

    public async Task<bool> IsCorrectAlbumId(long id, CancellationToken cancellationToken) =>
        await context.Albums.AnyAsync(a => a.Id == id, cancellationToken);

    public async Task<bool> IsCorrectTrackId(long id, CancellationToken cancellationToken) =>
        await context.Tracks.AnyAsync(a => a.Id == id, cancellationToken);

    public async Task<bool> IsCorrectPlaylistId(long id, CancellationToken cancellationToken) =>
        await context.Playlists.AnyAsync(a => a.Id == id, cancellationToken);

    public async Task<bool> IsCorrectGenreId(long id, CancellationToken cancellationToken) =>
        await context.Genres.AnyAsync(a => a.Id == id, cancellationToken);

    public async Task<bool> IsAvailableGenreName(string name, CancellationToken cancellationToken) =>
        ! await context.Genres.AnyAsync(a => a.Name == name, cancellationToken);
}