using AutoMapper;
using AutoMapper.QueryableExtensions;
using SpotifyAPI.Services.Interfaces;
using SpotifyAPI.Models.Artist;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Pagination;
using Microsoft.AspNetCore.Authorization;
using SpotifyAPI.Services;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ArtistsController(
    DataContext context,
    IMapper mapper,
    IScopedIdentityService identityService,
    IValidator<ArtistCreateVm> createValidator,
    IValidator<ArtistUpdateVm> updateValidator,
    IArtistsControllerService service,
    IPaginationService<ArtistVm, ArtistFilterVm> pagination
    ) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        await identityService.InitCurrentUserAsync(this);

        var userId = identityService.User?.Id;

        var artists = await context.Artists
            .ProjectTo<ArtistVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        foreach ( var artist in artists )
        {
            artist.IsFollowed = context.Followers.Where(x => x.ArtistId == artist.Id && x.UserId == userId).FirstOrDefault() != null;
        }

        return Ok(artists);
    }

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] ArtistFilterVm vm)
    {
        try
        {
            await identityService.InitCurrentUserAsync(this);

            var userId = identityService.User?.Id;

            var page = await pagination.GetPageAsync(vm);

            foreach (var artist in page.Data)
            {
                artist.IsFollowed = context.Followers.Where(x => x.ArtistId == artist.Id && x.UserId == userId).FirstOrDefault() != null;
            }

            return Ok(page);
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

        var artist = await context.Artists
            .ProjectTo<ArtistVm>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (artist is null)
            return NotFound();

        artist.IsFollowed = context.Followers.Where(x => x.ArtistId == artist.Id && x.UserId == userId).FirstOrDefault() != null;

        return Ok(artist);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromForm] ArtistCreateVm vm)
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
    public async Task<IActionResult> Update([FromForm] ArtistUpdateVm vm)
    {
        var validationResult = await updateValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await service.UpdateAsync(vm);

        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        await service.DeleteIfExistsAsync(id);

        return Ok();
    }
}