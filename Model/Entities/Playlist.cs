using Model.Entities.Identity;

namespace Model.Entities;

public class Playlist
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;

    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public ICollection<PlaylistTrack> Tracks { get; set; } = null!;
}