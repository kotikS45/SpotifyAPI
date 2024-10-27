namespace SpotifyAPI.Models.Artist;

public class ArtistVm
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public int AlbumsCount { get; set; }
    public int TracksCount { get; set; }
}
