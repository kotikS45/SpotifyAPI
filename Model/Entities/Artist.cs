namespace Model.Entities;

public class Artist
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;

    public ICollection<Album> Albums { get; set; } = null!;
    public ICollection<Follower> Followers { get; set; } = null!;
}