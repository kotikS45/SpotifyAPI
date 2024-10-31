using AutoMapper;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Album;

namespace SpotifyAPI.Services.Pagination;

public class AlbumPaginationService(
    DataContext context,
    IMapper mapper
    ) : PaginationService<Album, AlbumVm, AlbumFilterVm>(mapper)
{
    protected override IQueryable<Album> GetQuery() => context.Albums.OrderBy(c => c.Id);

    protected override IQueryable<Album> FilterQuery(IQueryable<Album> query, AlbumFilterVm paginationVm)
    {
        if (paginationVm.isRandom == true)
        {
            query = query.OrderBy(c => Guid.NewGuid());
        }
        else
        {
            query = query.OrderBy(c => c.Id);
        }

        if (paginationVm.Name is not null)
            query = query.Where(c => c.Name.ToLower().Contains(paginationVm.Name.ToLower()));

        if (paginationVm.ArtistId is not null)
            query = query.Where(c => c.ArtistId == paginationVm.ArtistId);

        return query;
    }
}