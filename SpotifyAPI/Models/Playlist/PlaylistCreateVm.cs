namespace SpotifyAPI.Models.Playlist;

public class PlaylistCreateVm
{
    public string Name { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
}