using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Like;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services.Pagination;

public class LikePaginationService(
    DataContext context,
    IScopedIdentityService identityService,
    IMapper mapper
    ) : PaginationService<Like, TrackVm, LikeFilterVm>(mapper)
{
    protected override IQueryable<Like> GetQuery() => context.Likes;

    protected override IQueryable<Like> FilterQuery(IQueryable<Like> query, LikeFilterVm paginationVm)
    {
        query = query.Where(l => l.UserId == identityService.GetRequiredUser().Id);

        return query;
    }

    protected override async Task<IEnumerable<TrackVm>> MapAsync(IQueryable<Like> query)
    {
        return await query
            .Select(l => l.Track)
            .ProjectTo<TrackVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();
    }
}