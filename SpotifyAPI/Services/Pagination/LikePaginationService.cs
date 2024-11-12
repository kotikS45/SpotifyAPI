using AutoMapper;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Like;
using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Services.Pagination;

public class LikePaginationService(
    DataContext context,
    IMapper mapper
    ) : PaginationService<Track, TrackVm, LikeFilterVm>(mapper)
{
    protected override IQueryable<Track> GetQuery() => context.Tracks.OrderBy(c => c.Id);

    protected override IQueryable<Track> FilterQuery(IQueryable<Track> query, LikeFilterVm paginationVm)
    {
        var user = context.Users.First(x => x.UserName == paginationVm.Username);

        var likedTracksIds = context.Likes
            .Where(f => f.UserId == user.Id)
            .Select(f => f.TrackId);

        query = query.Where(a => likedTracksIds.Contains(a.Id));

        if (paginationVm.Name is not null)
            query = query.Where(c => c.Name.ToLower().Contains(paginationVm.Name.ToLower()));

        return query;
    }
}