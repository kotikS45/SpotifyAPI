using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class TracksController(
    DataContext context,
    IScopedIdentityService identityService,
    IValidator<TrackCreateVm> createValidator,
    IValidator<TrackUpdateVm> updateValidator,
    ITrackControllerService service,
    IPaginationService<TrackVm, TrackFilterVm> pagination
    ) : ControllerBase 
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        await identityService.InitCurrentUserAsync(this);

        var userId = identityService.User?.Id;
        var likedTrackIds = userId != null
            ? await context.Likes
                .Where(l => l.UserId == userId)
                .Select(l => l.TrackId)
                .ToListAsync()
            : new List<long>();

        var tracks = await context.Tracks
            .Include(t => t.Genres)
                .ThenInclude(tg => tg.Genre)
            .Select(t => new TrackVm
            {
                Id = t.Id,
                Name = t.Name,
                Duration = t.Duration,
                AlbumId = t.AlbumId,
                ArtistId = t.Album.ArtistId,
                AlbumName = t.Album.Name,
                ArtistName = t.Album.Artist.Name,
                IsLiked = likedTrackIds.Contains(t.Id),
                Path = t.Path,
                Genres = t.Genres.Select(g => new GenreVm { Id = g.Genre.Id, Name = g.Genre.Name, Image = g.Genre.Image }),
                Image = t.Image
            })
            .ToArrayAsync();

        return Ok(tracks);
    }

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] TrackFilterVm vm)
    {
        try
        {
            await identityService.InitCurrentUserAsync(this);

            var userId = identityService.User?.Id;
            var likedTrackIds = userId != null
                ? await context.Likes
                    .Where(l => l.UserId == userId)
                    .Select(l => l.TrackId)
                    .ToListAsync()
                : new List<long>();

            var tracks = await pagination.GetPageAsync(vm);

            foreach (var track in tracks.Data)
            {
                track.IsLiked = likedTrackIds.Contains(track.Id);
            }

            return Ok(tracks);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        await identityService.InitCurrentUserAsync(this);

        var userId = identityService.User?.Id;
        var isLiked = userId != null && await context.Likes
            .AnyAsync(l => l.UserId == userId && l.TrackId == id);

        var track = await context.Tracks
            .Include(t => t.Genres)
            .ThenInclude(tg => tg.Genre)
            .Select(t => new TrackVm
            {
                Id = t.Id,
                Name = t.Name,
                Duration = t.Duration,
                AlbumId = t.AlbumId,
                ArtistId = t.Album.ArtistId,
                AlbumName = t.Album.Name,
                ArtistName = t.Album.Artist.Name,
                Path = t.Path,
                Genres = t.Genres.Select(g => new GenreVm { Id = g.Genre.Id, Name = g.Genre.Name }).ToList(),
                IsLiked = isLiked
            })
            .FirstOrDefaultAsync(c => c.Id == id);

        if (track is null)
            return NotFound();

        return Ok(track);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(TrackCreateVm vm)
    {
        var validationResult = await createValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await service.CreateAsync(vm);

        return Ok();
    }

    [HttpPut]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(TrackUpdateVm vm)
    {
        var validationResult = await updateValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await service.UpdateAsync(vm);

        return Ok();
    }

    [HttpDelete("id")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        await service.DeleteIfExistsAsync(id);
        return Ok();
    }
}