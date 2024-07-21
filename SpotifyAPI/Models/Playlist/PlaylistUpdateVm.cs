namespace SpotifyAPI.Models.Playlist;

public class PlaylistUpdateVm
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
}