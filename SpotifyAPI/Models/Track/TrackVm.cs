namespace SpotifyAPI.Models.Track;

public class TrackVm
{
    public long id { get; set; }
    public int Duration { get; set; }
    public long AlbumId { get; set; }
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
}