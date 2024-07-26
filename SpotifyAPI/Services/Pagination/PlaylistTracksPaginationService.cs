using AutoMapper;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.PlaylistTrack;
using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Services.Pagination;
public class PlaylistTracksPaginationService(
    DataContext context,
    IMapper mapper
    ) : PaginationService<Track, TrackVm, PlaylistTrackFilterVm>(mapper)
{
    protected override IQueryable<Track> GetQuery() => context.Tracks.OrderBy(c => c.Id);

    protected override IQueryable<Track> FilterQuery(IQueryable<Track> query, PlaylistTrackFilterVm paginationVm)
    {
        if (paginationVm.Name is not null)
            query = query.Where(c => c.Name.ToLower().Contains(paginationVm.Name.ToLower()));

        if (paginationVm.AlbumId is not null)
            query = query.Where(c => c.AlbumId == paginationVm.AlbumId);

        if (paginationVm.ArtistId is not null)
        {
            var albumIds = context.Albums
                .Where(a => a.ArtistId == paginationVm.ArtistId)
                .Select(a => a.Id);

            query = query.Where(t => albumIds.Contains(t.AlbumId));
        }

        var trackIdsInPlaylist = context.PlaylistTracks
            .Where(pt => pt.PlaylistId == paginationVm.PlaylistId)
            .Select(pt => pt.TrackId);

        query = query.Where(t => trackIdsInPlaylist.Contains(t.Id));

        return query;
    }
}