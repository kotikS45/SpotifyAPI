namespace SpotifyAPI.Models.Album;

public class AlbumUpdateVm
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }
    public long ArtistId { get; set; }
}