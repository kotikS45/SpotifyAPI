using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.PlaylistTrack;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PlaylistTracksController(
    DataContext context,
    IIdentityService identityService,
    IMapper mapper,
    IValidator<PlaylistTrackCreateVm> createValidator,
    IValidator<PlaylistTrackDeleteVm> deleteValidator,
    IPlaylistTrackControllerService service,
    IPaginationService<TrackVm, PlaylistTrackFilterVm> pagination) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetById(long id)
    {
        var user = await identityService.GetCurrentUserAsync(this);
        var playlist = await context.Playlists.FirstOrDefaultAsync(x => x.Id == id);

        if (playlist == null)
            return NotFound();

        if (playlist.UserId != user.Id)
            return Forbid("The playlist is not own");

        var tracks = await context.PlaylistTracks
            .Where(pt => pt.PlaylistId == id)
            .Select(pt => pt.Track)
            .ProjectTo<TrackVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        return Ok(tracks);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetPage([FromQuery] PlaylistTrackFilterVm vm)
    {
        var user = await identityService.GetCurrentUserAsync(this);
        var playlist = await context.Playlists.FirstOrDefaultAsync(x => x.Id == vm.PlaylistId);

        if (playlist == null)
            return NotFound();

        if (playlist.UserId != user.Id)
            return Forbid("The playlist is not own");

        try
        {
            return Ok(await pagination.GetPageAsync(vm));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Create([FromForm] PlaylistTrackCreateVm vm)
    {
        var validationResult = await createValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var user = await identityService.GetCurrentUserAsync(this);

        var playlist = await context.Playlists.FirstAsync(x => x.Id == vm.PlaylistId);

        if (playlist.UserId != user.Id)
            return Forbid("The playlist is not own");

        await service.CreateAsync(vm);

        return Ok();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Delete([FromForm] PlaylistTrackDeleteVm vm)
    {
        var validationResult = await deleteValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var playlist = await context.Playlists.FirstAsync(x => x.Id == vm.PlaylistId);

        var user = await identityService.GetCurrentUserAsync(this);

        if (playlist.UserId != user.Id)
            return Forbid("The playlist is not own");

        await service.DeleteIfExistsAsync(vm);

        return Ok();
    }
}