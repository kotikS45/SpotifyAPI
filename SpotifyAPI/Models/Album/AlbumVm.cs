using SpotifyAPI.Models.Artist;

namespace SpotifyAPI.Models.Album;

public class AlbumVm
{
    public long Id { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public ArtistVm Artist { get; set; } = null!;
}