using Model.Entities;
using SpotifyAPI.Models.Artist;

namespace SpotifyAPI.Services.Interfaces;

public interface IArtistsControllerService
{
    Task CreateAsync(ArtistCreateVm vm);
}