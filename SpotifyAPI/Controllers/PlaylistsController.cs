using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class PlaylistsController(
    DataContext context,
    IMapper mapper,
    IValidator<PlaylistCreateVm> createValidator,
    IValidator<PlaylistUpdateVm> updateValidator,
    IPlaylistControllerService service,
    IIdentityService identityService,
    IPaginationService<PlaylistVm, PlaylistFilterVm> pagination
    ) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAll()
    {
        var entities = await context.Playlists
            .ProjectTo<PlaylistVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        return Ok(entities);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetPage([FromQuery] PlaylistFilterVm vm)
    {
        try
        {
            return Ok(await pagination.GetPageAsync(vm));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetById(long id)
    {
        var user = await identityService.GetCurrentUserAsync(this);

        var entity = await context.Playlists
            .Where(b => b.UserId == user.Id)
            .ProjectTo<PlaylistVm>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (entity is null)
            return NotFound();

        return Ok(entity);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Create([FromForm] PlaylistCreateVm vm)
    {
        var validationResult = await createValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var user = await identityService.GetCurrentUserAsync(this);

        await service.CreateAsync(vm, user.Id);

        return Ok();
    }

    [HttpPut]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Update([FromForm] PlaylistUpdateVm vm)
    {
        var validationResult = await updateValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var playlist = await context.Playlists.FirstAsync(b => b.Id == vm.Id);
        var user = await identityService.GetCurrentUserAsync(this);
        if (playlist.UserId != user.Id)
            return Forbid("The playlist is not own");

        await service.UpdateAsync(vm);

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Delete(long id)
    {
        var playlist = await context.Playlists.FirstOrDefaultAsync(b => b.Id == id);

        if (playlist is not null)
        {
            var user = await identityService.GetCurrentUserAsync(this);

            if (playlist.UserId != user.Id)
                return Forbid("The playlist is not own");

            await service.DeleteIfExistsAsync(id);
        }

        return Ok();
    }
}