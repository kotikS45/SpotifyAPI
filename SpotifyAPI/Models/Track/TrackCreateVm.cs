namespace SpotifyAPI.Models.Track;

public class TrackCreateVm
{
    public string Name { get; set; } = null!;
    public IFormFile Audio { get; set; } = null!;
    public long AlbumId { get; set; }
}