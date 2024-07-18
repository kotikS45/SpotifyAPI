using Model.Entities.Identity;

namespace Model.Entities;

public class Follower
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long ArtistId { get; set; }
    public Artist Artist { get; set; } = null!;
}