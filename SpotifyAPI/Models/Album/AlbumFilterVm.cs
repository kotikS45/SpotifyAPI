using SpotifyAPI.Models.Pagination;

namespace SpotifyAPI.Models.Album;

public class AlbumFilterVm : PaginationVm
{
    public string? Name { get; set; }
    public long? ArtistId { get; set; }
}