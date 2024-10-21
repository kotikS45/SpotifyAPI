using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class TracksController(
    DataContext context,
    IValidator<TrackCreateVm> createValidator,
    IValidator<TrackUpdateVm> updateValidator,
    ITrackControllerService service,
    IPaginationService<TrackVm, TrackFilterVm> pagination
    ) : ControllerBase 
{

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var tracks = await context.Tracks
            .Include(t => t.Genres)
                .ThenInclude(tg => tg.Genre)
            .Select(t => new TrackVm
            {
                Id = t.Id,
                Name = t.Name,
                Duration = t.Duration,
                AlbumId = t.AlbumId,
                ArtistId = t.Album.ArtistId,
                AlbumName = t.Album.Name,
                ArtistName = t.Album.Artist.Name,
                Path = t.Path,
                Genres = t.Genres.Select(g => new GenreVm { Id = g.Genre.Id, Name = g.Genre.Name, Image = g.Genre.Image }),
                Image = t.Image
            })
            .ToArrayAsync();

        return Ok(tracks);
    }

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] TrackFilterVm vm)
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
    public async Task<IActionResult> GetById(long id)
    {
        var track = await context.Tracks
            .Include(t => t.Genres)
            .ThenInclude(tg => tg.Genre)
            .Select(t => new TrackVm
            {
                Id = t.Id,
                Name = t.Name,
                Duration = t.Duration,
                AlbumId = t.AlbumId,
                ArtistId = t.Album.ArtistId,
                AlbumName = t.Album.Name,
                ArtistName = t.Album.Artist.Name,
                Path = t.Path,
                Genres = t.Genres.Select(g => new GenreVm { Id = g.Genre.Id, Name = g.Genre.Name }).ToList()
            })
            .FirstOrDefaultAsync(c => c.Id == id);

        if (track is null)
            return NotFound();

        return Ok(track);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(TrackCreateVm vm)
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
    public async Task<IActionResult> Update(TrackUpdateVm vm)
    {
        var validationResult = await updateValidator.ValidateAsync(vm);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await service.UpdateAsync(vm);

        return Ok();
    }

    [HttpDelete("id")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(long id)
    {
        await service.DeleteIfExistsAsync(id);
        return Ok();
    }
}