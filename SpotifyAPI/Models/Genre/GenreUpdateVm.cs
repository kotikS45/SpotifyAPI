namespace SpotifyAPI.Models.Genre;

public class GenreUpdateVm
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
}