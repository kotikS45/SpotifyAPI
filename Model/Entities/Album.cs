namespace Model.Entities;

public class Album
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public DateTime ReleaseDate { get; set; }

    public long ArtistId { get; set; }
    public Artist Artist { get; set; } = null!;

    public ICollection<Track> Tracks { get; set; } = null!;
}