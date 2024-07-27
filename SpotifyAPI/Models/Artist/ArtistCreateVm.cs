namespace SpotifyAPI.Models.Artist;

public class ArtistCreateVm
{
    public string Name { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
}
