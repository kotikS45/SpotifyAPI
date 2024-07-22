using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Follower;

namespace SpotifyAPI.Services.Pagination;

public class FollowerPaginationService(
    DataContext context,
    IMapper mapper
    ) : PaginationService<Artist, ArtistVm, FollowerFilterVm>(mapper)
{
    protected override IQueryable<Artist> GetQuery() => context.Artists.OrderBy(c => c.Id);

    protected override IQueryable<Artist> FilterQuery(IQueryable<Artist> query, FollowerFilterVm paginationVm)
    {
        var user = context.Users.First(x => x.UserName == paginationVm.Username);

        var followedArtistIds = context.Followers
            .Where(f => f.UserId == user.Id)
            .Select(f => f.ArtistId);

        query = query.Where(a => followedArtistIds.Contains(a.Id));

        if (paginationVm.Name is not null)
            query = query.Where(c => c.Name.ToLower().Contains(paginationVm.Name.ToLower()));

        return query;
    }
}