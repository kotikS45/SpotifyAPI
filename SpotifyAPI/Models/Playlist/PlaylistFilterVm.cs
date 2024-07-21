using SpotifyAPI.Models.Pagination;

namespace SpotifyAPI.Models.Playlist;

public class PlaylistFilterVm : PaginationVm
{
    public string? Name { get; set; }
}