using AutoMapper;
using AutoMapper.QueryableExtensions;
using SpotifyAPI.Services.Interfaces;
using SpotifyAPI.Models.Artist;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ArtistsController(
    DataContext context,
    IMapper mapper,
    IValidator<ArtistCreateVm> createValidator,
    IArtistsControllerService service
    ) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var artists = await context.Artists
            .ProjectTo<ArtistVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        return Ok(artists);
    }

    [HttpPost]
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await service.DeleteIfExistsAsync(id);

        return Ok();
    }
}