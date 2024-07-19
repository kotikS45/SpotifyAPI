﻿using AutoMapper;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Services.Pagination;

public class TrackPaginationService(
    DataContext context,
    IMapper mapper
    ) : PaginationService<Track, TrackVm, TrackFilterVm>(mapper)
{
    protected override IQueryable<Track> GetQuery() => context.Tracks.OrderBy(c => c.Id);

    protected override IQueryable<Track> FilterQuery(IQueryable<Track> query, TrackFilterVm paginationVm)
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

        return query;
    }
}