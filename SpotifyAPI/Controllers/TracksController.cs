using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class TracksController(
    DataContext context,
    IMapper mapper,
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
            .ProjectTo<TrackVm>(mapper.ConfigurationProvider)
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
        var tracks = await context.Tracks
            .ProjectTo<TrackVm>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (tracks is null)
            return NotFound();

        return Ok(tracks);
    }

    [HttpPost]
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
    public async Task<IActionResult> Delete(long id)
    {
        await service.DeleteIfExistsAsync(id);
        return Ok();
    }
}