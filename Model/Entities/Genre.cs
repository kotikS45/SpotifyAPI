namespace Model.Entities;

public class Genre
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;

    public ICollection<TrackGenre> Tracks { get; set; } = null!;
}