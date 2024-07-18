namespace Model.Entities;

public class PlaylistTrack
{
    public long PlaylistId { get; set; }
    public Playlist Playlist { get; set; } = null!;

    public long TrackId { get; set; }
    public Track Track { get; set; } = null!;
}