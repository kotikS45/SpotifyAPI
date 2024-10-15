namespace Model.Entities;

public class Track
{
    public long Id { get; set; }
    public int Duration { get; set; }
    public string Name { get; set; } = null!;
    public string Path { get; set; } = null!;
    public string Image { get; set; } = null!;

    public long AlbumId { get; set; }
    public Album Album { get; set; } = null!;

    public ICollection<PlaylistTrack> Playlists { get; set; } = null!;
    public ICollection<Like> Likes { get; set; } = null!;
    public ICollection<TrackGenre> Genres { get; set; } = null!;
}