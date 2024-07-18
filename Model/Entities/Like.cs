using Model.Entities.Identity;

namespace Model.Entities;

public class Like
{
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public long TrackId { get; set; }
    public Track Track { get; set; } = null!;

    public DateTime LikeDateTime { get; set; }
}