using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Like;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class LikesController(
    DataContext context,
    IMapper mapper,
    IScopedIdentityService identityService,
    ILikeControllerService service,
    IValidator<LikeVm> validator,
    IPaginationService<TrackVm, LikeFilterVm> pagination) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAll()
    {
        await identityService.InitCurrentUserAsync(this);

        if (identityService.User == null)
            return NotFound();

        var artists = await context.Likes
            .Where(x => x.UserId == identityService.User.Id)
            .Select(x => new TrackVm
            {
                Id = x.Track.Id,
                Duration = x.Track.Duration,
                AlbumId = x.Track.AlbumId,
                ArtistId = x.Track.Album.ArtistId,
                IsLiked = context.Likes.Any(l => l.UserId == identityService.User.Id && l.TrackId == x.TrackId),
                Name = x.Track.Name,
                Path = x.Track.Path,
                Image = x.Track.Image,
                AlbumName = x.Track.Album.Name,
                ArtistName = x.Track.Album.Artist.Name
            })
            .ToArrayAsync();

        return Ok(artists);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetPage([FromQuery] LikeFilterVm vm)
    {
        try
        {
            await identityService.InitCurrentUserAsync(this);

            if (identityService.User == null)
                return NotFound();

            var pageResult = await pagination.GetPageAsync(vm);

            foreach (var track in pageResult.Data)
            {
                track.IsLiked = await context.Likes
                    .AnyAsync(l => l.UserId == identityService.User.Id && l.TrackId == track.Id);
            }

            return Ok(pageResult);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{trackId}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Like(long trackId)
    {
        await identityService.InitCurrentUserAsync(this);

        if (identityService.User == null)
            return NotFound();

        var track = await context.Tracks.FindAsync(trackId);
        if (track == null)
            return NotFound("Track not found");

        await service.Like(identityService.User.Id, trackId);

        return Ok();
    }

    [HttpDelete("{trackId}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Unlike(long trackId)
    {
        await identityService.InitCurrentUserAsync(this);

        if (identityService.User == null)
            return NotFound();

        var track = await context.Tracks.FindAsync(trackId);
        if (track == null)
            return NotFound("Track not found");

        await service.Unlike(identityService.User.Id, trackId);

        return Ok();
    }
}