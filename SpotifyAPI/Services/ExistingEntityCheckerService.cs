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
}