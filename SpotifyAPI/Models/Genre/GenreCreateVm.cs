namespace SpotifyAPI.Models.Genre;

public class GenreCreateVm
{
    public string Name { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
}