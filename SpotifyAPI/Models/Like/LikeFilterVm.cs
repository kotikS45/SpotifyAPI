﻿using SpotifyAPI.Models.Pagination;

namespace SpotifyAPI.Models.Like;

public class LikeFilterVm : PaginationVm
{
    public string? Name { get; set; }
    public string Username { get; set; } = null!;
}