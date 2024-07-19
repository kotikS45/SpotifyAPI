namespace SpotifyAPI.Models.Album;

public class AlbumCreateVm
{
    public string Name { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public long ArtistId { get; set; }
}