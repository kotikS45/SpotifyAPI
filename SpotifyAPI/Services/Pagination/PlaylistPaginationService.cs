using AutoMapper;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Playlist;

namespace SpotifyAPI.Services.Pagination;

public class PlaylistPaginationService(
    DataContext context,
    IMapper mapper
    ) : PaginationService<Playlist, PlaylistVm, PlaylistFilterVm>(mapper)
{
    protected override IQueryable<Playlist> GetQuery() => context.Playlists.OrderBy(c => c.Id);

    protected override IQueryable<Playlist> FilterQuery(IQueryable<Playlist> query, PlaylistFilterVm paginationVm)
    {
        if (paginationVm.Name is not null)
            query = query.Where(c => c.Name.ToLower().Contains(paginationVm.Name.ToLower()));

        return query;
    }
}