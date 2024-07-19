using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class TracksController(
    DataContext context,
    IMapper mapper
    ) : ControllerBase 
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tracks = await context.Tracks
            .ProjectTo<TrackVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        return Ok(tracks);
    }
}
