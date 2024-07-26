using SpotifyAPI.Models.Pagination;

namespace SpotifyAPI.Models.Follower;

public class FollowerFilterVm : PaginationVm
{
    public string? Name { get; set; }
    public string Username { get; set; } = null!;
}