using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class GenreControllerService(
    DataContext context,
    IMapper mapper,
    IImageService imageService) : IGenreControllerService
{
    public async Task CreateAsync(GenreCreateVm vm)
    {
        var genre = mapper.Map<Genre>(vm);
        genre.Image = await imageService.SaveImageAsync(vm.Image);

        await context.Genres.AddAsync(genre);

        try
        {
            await context.SaveChangesAsync();
        } catch (Exception)
        {
            imageService.DeleteImageIfExists(genre.Image);
            throw;
        }
    }

    public async Task UpdateAsync(GenreUpdateVm vm)
    {
        var genre = await context.Genres.FirstAsync(x => x.Id == vm.Id);

        genre.Name = vm.Name;

        string oldImage = genre.Image;
        genre.Image = await imageService.SaveImageAsync(vm.Image);
        try
        {
            await context.SaveChangesAsync();
            imageService.DeleteImageIfExists(oldImage);

        } catch (Exception)
        {
            imageService.DeleteImageIfExists(genre.Image);
            throw;
        }
    }

    public async Task DeleteIfExistsAsync(long id)
    {
        var genre = await context.Genres.FirstOrDefaultAsync(x => x.Id == id);

        if (genre is null)
            return;

        context.Genres.Remove(genre);
        await context.SaveChangesAsync();

        imageService.DeleteImageIfExists(genre.Image);
    }
}