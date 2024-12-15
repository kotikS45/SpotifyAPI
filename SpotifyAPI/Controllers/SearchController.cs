using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Album;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Models.Search;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class SearchController(
    DataContext context,
    IMapper mapper,
    IScopedIdentityService scopedIdentityService,
    IValidator<SearchVm> validator,
    IPaginationService<TrackVm, TrackFilterVm> trackPagination,
    IPaginationService<ArtistVm, ArtistFilterVm> artistPagination,
    IPaginationService<AlbumVm, AlbumFilterVm> albumPagination,
    IPaginationService<PlaylistVm, PlaylistFilterVm> playlistPagination) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetPageByName([FromQuery] SearchVm vm)
    {
        var validationResult = await validator.ValidateAsync(vm);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        await scopedIdentityService.InitCurrentUserAsync(this);

        var userId = scopedIdentityService.User?.Id;
        var likedTrackIds = userId != null
            ? await context.Likes
                .Where(l => l.UserId == userId)
                .Select(l => l.TrackId)
                .ToListAsync()
            : new List<long>();

        try
        {
            if (vm.Search != null)
                vm.Search = vm.Search.ToLower();

            var tracks = (await trackPagination.GetPageAsync(
                new TrackFilterVm
                {
                    PageIndex = 0,
                    PageSize = vm.TracksCount,
                    isRandom = true,
                    Name = vm.Search
                })).Data.ToList();

            foreach (var track in tracks)
            {
                track.IsLiked = likedTrackIds.Contains(track.Id);
            }

            var artists = (await artistPagination.GetPageAsync(
                new ArtistFilterVm
                {
                    PageIndex = 0,
                    PageSize = vm.ArtistCount,
                    isRandom = true,
                    Name = vm.Search
                })).Data.Take(vm.ArtistCount).ToList();

            foreach (var artist in artists)
            {
                artist.IsFollowed = context.Followers.Where(x => x.ArtistId == artist.Id && x.UserId == userId).FirstOrDefault() != null;
            }

            var albums = (await albumPagination.GetPageAsync(
                new AlbumFilterVm
                {
                    PageIndex = 0,
                    PageSize = vm.AlbumsCount,
                    isRandom = true,
                    Name = vm.Search
                })).Data.ToList();

            var playlists = (await playlistPagination.GetPageAsync(
                new PlaylistFilterVm
                {
                    PageIndex = 0,
                    PageSize = vm.PlaylistsCount,
                    isRandom = true,
                    Name = vm.Search
                })).Data.ToList();

            ArtistVm? bestArtist = null;
            AlbumVm? bestAlbum = null;
            TrackVm? bestTrack = null;

            if (artists.Any())
            {
                bestArtist = artists
                    .OrderBy(a => a.Name.Length)
                    .FirstOrDefault();
            }

            if (albums.Any())
            {
                bestAlbum = albums
                    .OrderBy(a => a.Name.Length)
                    .FirstOrDefault();
            }

            if (tracks.Any())
            {
                bestTrack = tracks
                    .OrderBy(t => t.Name.Length)
                    .FirstOrDefault();
            }

            var response = new SearchResponseVm
            {
                Artist = bestArtist,
                Album = bestAlbum,
                Track = bestTrack,
                ArtistsAvailable = artists.Count(),
                AlbumsAvailable = albums.Count(),
                TracksAvailable = tracks.Count(),
                PlaylistsAvailable = playlists.Count(),
                Artists = artists.Select(mapper.Map<ArtistVm>),
                Albums = albums.Select(mapper.Map<AlbumVm>),
                Tracks = tracks.Select(mapper.Map<TrackVm>),
                Playlists = playlists.Select(mapper.Map<PlaylistVm>)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetPageByGenre([FromQuery] SearchVm vm)
    {
        var validationResult = await validator.ValidateAsync(vm);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        await scopedIdentityService.InitCurrentUserAsync(this);

        var userId = scopedIdentityService.User?.Id;
        var likedTrackIds = userId != null
            ? await context.Likes
                .Where(l => l.UserId == userId)
                .Select(l => l.TrackId)
                .ToListAsync()
            : new List<long>();

        try
        {
            if (vm.GenreId.HasValue)
            {
                var tracksByGenre = (await trackPagination.GetPageAsync(
                    new TrackFilterVm { 
                        PageIndex = 0, 
                        PageSize = vm.TracksCount,
                        isRandom = true,
                        GenreId = vm.GenreId
                    })).Data.ToList();

                foreach (var track in tracksByGenre)
                {
                    track.IsLiked = likedTrackIds.Contains(track.Id);
                }

                var albumIds = tracksByGenre
                    .Select(t => t.AlbumId)
                    .Distinct()
                    .ToList();

                var albumsByGenre = context.Albums
                    .ProjectTo<AlbumVm>(mapper.ConfigurationProvider)
                    .Where(a => albumIds.Contains(a.Id))
                    .Take(vm.AlbumsCount)
                    .ToList();

                var artistIds = albumsByGenre
                    .Select(a => a.Artist.Id)
                    .Distinct()
                    .ToList();

                var artistsByGenre = context.Artists
                    .ProjectTo<ArtistVm>(mapper.ConfigurationProvider)
                    .Where(a => artistIds.Contains(a.Id))
                    .Take(vm.ArtistCount)
                    .ToList();


                foreach (var artist in artistsByGenre)
                {
                    artist.IsFollowed = context.Followers.Where(x => x.ArtistId == artist.Id && x.UserId == userId).FirstOrDefault() != null;
                }

                await scopedIdentityService.InitCurrentUserAsync(this);

                if (scopedIdentityService.User == null)
                    return NotFound();

                var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == scopedIdentityService.User.UserName);

                if (user == null)
                    return NotFound();

                var playlistsByGenre = context.Playlists
                    .Where(p => p.UserId == user.Id && p.Tracks.Any(t => t.Track.Genres.Any(g => g.GenreId == vm.GenreId)))
                    .Take(vm.PlaylistsCount)
                    .Select(mapper.Map<PlaylistVm>)
                    .ToList();

                var genreResponse = new SearchResponseVm
                {
                    ArtistsAvailable = artistsByGenre.Count,
                    AlbumsAvailable = albumsByGenre.Count,
                    TracksAvailable = tracksByGenre.Count,
                    PlaylistsAvailable = playlistsByGenre.Count,
                    Artists = artistsByGenre,
                    Albums = albumsByGenre,
                    Tracks = tracksByGenre,
                    Playlists = playlistsByGenre
                };

                return Ok(genreResponse);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}