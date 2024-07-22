using SpotifyAPI.Models.Pagination;

namespace SpotifyAPI.Models.PlaylistTrack;

public class PlaylistTrackFilterVm : PaginationVm
{
    public string? Name { get; set; }
    public long? AlbumId { get; set; }
    public long? ArtistId { get; set; }
    public long PlaylistId { get; set; }
}