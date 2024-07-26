using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Models.Playlist;

public class PlaylistVm
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;

    //public IEnumerable<TrackVm> Tracks { get; set; } = null!;
}