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
    IIdentityService identityService,
    IFollowerControllerService service,
    IValidator<FollowerVm> validator,
    IPaginationService<ArtistVm, FollowerFilterVm> pagination) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetAll([FromQuery] string username)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == username);

        if (user == null)
            return NotFound();

        var artists = await context.Followers
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Artist)
            .ProjectTo<ArtistVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        return Ok(artists);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetPage([FromQuery] FollowerFilterVm vm)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username);

            if (user == null)
                return NotFound();

            return Ok(await pagination.GetPageAsync(vm));
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