using SpotifyAPI.Models.Pagination;

namespace SpotifyAPI.Models.Like;

public class LikeFilterVm : PaginationVm
{
    public string? Name { get; set; }
    public string Username { get; set; } = null!;
}
/*
namespace SpotifyAPI.Models.Follower
{
    public class FollowerFilterVm : BaseFilterVm
    {
        // Можна додати додаткові поля для специфічних фільтрів по Follower
    }
}

namespace SpotifyAPI.Models.Like
{
    public class LikeFilterVm : BaseFilterVm
    {
        // Можна додати додаткові поля для специфічних фільтрів по Like
    }
}

*/
