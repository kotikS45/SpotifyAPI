using SpotifyAPI.Models.Pagination;

namespace SpotifyAPI.Models.Artist;

public class ArtistFilterVm : PaginationVm
{
    public string? Name { get; set; }
}
