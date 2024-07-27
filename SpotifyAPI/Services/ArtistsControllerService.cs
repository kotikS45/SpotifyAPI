using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class ArtistsControllerService(
    DataContext context,
    IMapper mapper,
    IImageService imageService
    ) : IArtistsControllerService
{
    public async Task CreateAsync(ArtistCreateVm vm)
    {
        var artist = mapper.Map<Artist>(vm);
        artist.Image = await imageService.SaveImageAsync(vm.Image);

        await context.Artists.AddAsync(artist);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            imageService.DeleteImageIfExists(artist.Image);
            throw;
        }
    }

    public async Task UpdateAsync(ArtistUpdateVm vm)
    {
        var artist = await context.Artists.FirstAsync(a => a.Id == vm.Id);

        string oldImage = artist.Image;

        artist.Name = vm.Name;
        artist.Image = await imageService.SaveImageAsync(vm.Image);

        try
        {
            await context.SaveChangesAsync();

            imageService.DeleteImageIfExists(oldImage);
        }
        catch (Exception)
        {
            imageService.DeleteImageIfExists(artist.Image);
            throw;
        }
    }

    public async Task DeleteIfExistsAsync(long id)
    {
        var artist = await context.Artists.FirstOrDefaultAsync(a => a.Id == id);

        if (artist is null)
        {
            return;
        }

        context.Artists.Remove(artist);
        await context.SaveChangesAsync();

        imageService.DeleteImageIfExists(artist.Image);
    }
}