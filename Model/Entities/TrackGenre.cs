namespace Model.Entities;

public class TrackGenre
{
    public long TrackId { get; set; }
    public Track Track { get; set; } = null!;

    public long GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
}