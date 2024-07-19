using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Services.Interfaces;

public interface ITrackControllerService
{
    Task CreateAsync(TrackCreateVm vm);
    Task UpdateAsync(TrackUpdateVm vm);
    Task DeleteIfExistsAsync(long id);
}