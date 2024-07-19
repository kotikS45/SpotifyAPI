using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Album;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AlbumsController(
    DataContext context,
    IMapper mapper,
    IValidator<AlbumCreateVm> createValidator,
    IAlbumsCotrollerService service
    ) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var albums = await context.Albums
            .ProjectTo<AlbumVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        return Ok(albums);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] AlbumCreateVm vm)
    {
        var validationResult = await createValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await service.CreateAsync(vm);

        return Ok();
    }
}