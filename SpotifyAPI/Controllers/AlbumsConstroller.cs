using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Album;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class AlbumsController(
    DataContext context,
    IMapper mapper
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
}