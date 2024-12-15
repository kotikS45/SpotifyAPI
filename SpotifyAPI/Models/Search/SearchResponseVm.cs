using SpotifyAPI.Models.Album;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Models.Search;

public class SearchResponseVm
{
    public ArtistVm? Artist { get; set; }
    public AlbumVm? Album { get; set; }
    public TrackVm? Track { get; set; }
    public int ArtistsAvailable { get; set; }
    public int AlbumsAvailable { get; set; }
    public int TracksAvailable { get; set; }
    public int PlaylistsAvailable { get; set; }
    public IEnumerable<ArtistVm> Artists { get; set; } = null!;
    public IEnumerable<AlbumVm> Albums { get; set; } = null!;
    public IEnumerable<TrackVm> Tracks { get; set; } = null!;
    public IEnumerable<PlaylistVm> Playlists { get; set; } = null!;
}