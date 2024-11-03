using AutoMapper;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Artist;

namespace SpotifyAPI.Services.Pagination;

public class ArtistPaginationService(
    DataContext context,
    IMapper mapper
    ) : PaginationService<Artist, ArtistVm, ArtistFilterVm>(mapper)
{
    protected override IQueryable<Artist> GetQuery() => context.Artists.OrderBy(c => c.Id);

    protected override IQueryable<Artist> FilterQuery(IQueryable<Artist> query, ArtistFilterVm paginationVm)
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

        return query;
    }
}
