using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Follower;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class FollowerController (
    DataContext context,
    IMapper mapper,
    IScopedIdentityService scopedIdentityService,
    IIdentityService identityService,
    IFollowerControllerService service,
    IValidator<FollowerVm> validator,
    IPaginationService<ArtistVm, FollowerFilterVm> pagination) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAll()
    {
        await scopedIdentityService.InitCurrentUserAsync(this);

        if (scopedIdentityService.User == null)
            return NotFound();

        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == scopedIdentityService.User.UserName);

        if (user == null)
            return NotFound();

        var artists = await context.Followers
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Artist)
            .ProjectTo<ArtistVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        foreach (var artist in artists)
        {
            artist.IsFollowed = context.Followers.Where(x => x.ArtistId == artist.Id && x.UserId == user.Id).FirstOrDefault() != null;
        }

        return Ok(artists);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetPage([FromQuery] FollowerFilterVm vm)
    {
        try
        {
            await scopedIdentityService.InitCurrentUserAsync(this);

            if (scopedIdentityService.User == null)
                return NotFound();

            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == scopedIdentityService.User.UserName);

            if (user == null)
                return NotFound();

            var page = await pagination.GetPageAsync(vm);

            foreach (var artist in page.Data)
            {
                artist.IsFollowed = context.Followers.Where(x => x.ArtistId == artist.Id && x.UserId == user.Id).FirstOrDefault() != null;
            }

            return Ok(page);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Follow([FromForm] FollowerVm vm)
    {
        var validationResult = await validator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var user = await identityService.GetCurrentUserAsync(this);
        await service.Follow(user.Id, vm);
        return Ok();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Unfollow([FromForm] FollowerVm vm)
    {
        var validationResult = await validator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var user = await identityService.GetCurrentUserAsync(this);
        await service.Unfollow(user.Id, vm);
        return Ok();
    }
}