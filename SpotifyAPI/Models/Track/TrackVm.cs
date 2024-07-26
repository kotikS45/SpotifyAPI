using SpotifyAPI.Models.Genre;

namespace SpotifyAPI.Models.Track;

public class TrackVm
{
    public long Id { get; set; }
    public int Duration { get; set; }
    public long AlbumId { get; set; }
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
    public IEnumerable<GenreVm> Genres { get; set; } = null!;
}