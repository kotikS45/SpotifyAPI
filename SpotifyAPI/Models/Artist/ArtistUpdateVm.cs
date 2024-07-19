namespace SpotifyAPI.Models.Artist;

public class ArtistUpdateVm
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Genre { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
}