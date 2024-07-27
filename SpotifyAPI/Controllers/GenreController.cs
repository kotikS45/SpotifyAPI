using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class GenreController(
    DataContext context,
    IMapper mapper,
    IValidator<GenreCreateVm> createValidator,
    IValidator<GenreUpdateVm> updateValidator,
    IGenreControllerService service
    ) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var artists = await context.Genres
            .ProjectTo<GenreVm>(mapper.ConfigurationProvider)
            .ToArrayAsync();

        return Ok(artists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var genre = await context.Genres
            .FirstOrDefaultAsync(g => g.Id == id);

        if (genre is null)
            return NotFound();

        var result = mapper.Map<GenreVm>(genre);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(GenreCreateVm vm)
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
    public async Task<IActionResult> Update(GenreUpdateVm vm)
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