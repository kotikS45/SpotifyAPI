using AutoMapper;
using AutoMapper.QueryableExtensions;
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
            .Select(x => x.Track)
            .ProjectTo<TrackVm>(mapper.ConfigurationProvider)
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

            return Ok(await pagination.GetPageAsync(vm));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Like([FromForm] LikeVm vm)
    {
        var validationResult = await validator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        await identityService.InitCurrentUserAsync(this);

        if (identityService.User == null)
            return NotFound();

        await service.Like(identityService.User.Id, vm);
        return Ok();
    }

    [HttpDelete]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> Unlike([FromForm] LikeVm vm)
    {
        var validationResult = await validator.ValidateAsync(vm);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);


        await identityService.InitCurrentUserAsync(this);

        if (identityService.User == null)
            return NotFound();

        await service.Unlike(identityService.User.Id, vm);
        return Ok();
    }
}