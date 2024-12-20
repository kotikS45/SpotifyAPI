    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using Model.Context;
    using Model.Entities;
    using SpotifyAPI.Models.Playlist;
    using SpotifyAPI.Models.Track;
    using SpotifyAPI.Services.Interfaces;

    namespace SpotifyAPI.Services.Pagination;

    public class PlaylistPaginationService(
        DataContext context,
        IScopedIdentityService identityService,
        IMapper mapper
        ) : PaginationService<Playlist, PlaylistVm, PlaylistFilterVm>(mapper)
    {
        protected override IQueryable<Playlist> GetQuery() => context.Playlists.OrderBy(c => c.Id);

        protected override IQueryable<Playlist> FilterQuery(IQueryable<Playlist> query, PlaylistFilterVm paginationVm)
        {
            long id = identityService.GetRequiredUser().Id;

            query = query.Where(i => i.UserId == id);

            if (paginationVm.Name is not null)
                query = query.Where(c => c.Name.ToLower().Contains(paginationVm.Name.ToLower()));

            return query;
        }
    }