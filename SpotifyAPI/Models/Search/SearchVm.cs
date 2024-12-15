namespace SpotifyAPI.Models.Search;

public class SearchVm
{
    public string? Search { get; set; } = null!;
    public long? GenreId { get; set; }
    public int ArtistCount { get; set; }
    public int AlbumsCount { get; set; }
    public int TracksCount { get; set; }
    public int PlaylistsCount { get; set; }
}