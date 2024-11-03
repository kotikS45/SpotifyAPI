namespace SpotifyAPI.Models.Pagination;

public class PaginationVm
{
    public int? PageIndex { get; set; }
    public int? PageSize { get; set; }
    public bool? isRandom { get; set; }
}