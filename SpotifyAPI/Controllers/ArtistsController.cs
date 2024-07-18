using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Artist;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class ArtistsController(
    DataContext context,
    IMapper mapper
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
}