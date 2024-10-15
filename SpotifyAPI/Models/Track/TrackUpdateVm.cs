using Model.Entities;

namespace SpotifyAPI.Models.Track;

public class TrackUpdateVm
{
    public long Id { get; set; }
    public long AlbumId { get; set; }
    public string Name { get; set; } = null!;
    public IFormFile Audio { get; set; } = null!;
    public ICollection<long> Genres { get; set; } = null!;
    public IFormFile Image { get; set; } = null!;
}