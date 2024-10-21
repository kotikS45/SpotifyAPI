namespace SpotifyAPI.Models.Track;

public class TrackCreateVm
{
    public long AlbumId { get; set; }
    public string Name { get; set; } = null!;
    public IFormFile Audio { get; set; } = null!;
    public ICollection<long> Genres { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
}