using SpotifyAPI.Models.Pagination;

namespace SpotifyAPI.Models.Track;

public class TrackFilterVm : PaginationVm
{
    public string? Name { get; set; }
    public long? AlbumId { get; set; }
    public long? ArtistId { get; set; }
}