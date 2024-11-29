using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Follower;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services.Pagination;

public class FollowerPaginationService(
    DataContext context,
    IMapper mapper,
    IScopedIdentityService identityService
    ) : PaginationService<Artist, ArtistVm, FollowerFilterVm>(mapper)
{
    protected override IQueryable<Artist> GetQuery() => context.Artists.OrderBy(c => c.Id);

    protected override IQueryable<Artist> FilterQuery(IQueryable<Artist> query, FollowerFilterVm paginationVm)
    {
        var username = identityService.GetRequiredUser().UserName;

        var user = context.Users.First(x => x.UserName == username);

        var followedArtistIds = context.Followers
            .Where(f => f.UserId == user.Id)
            .Select(f => f.ArtistId);

        query = query.Where(a => followedArtistIds.Contains(a.Id));

        if (paginationVm.Name is not null)
            query = query.Where(c => c.Name.ToLower().Contains(paginationVm.Name.ToLower()));

        return query;
    }
}