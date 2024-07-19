using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Album;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class AlbumsControllerService(
    DataContext context,
    IMapper mapper,
    IImageService imageService) : IAlbumsCotrollerService
{
    public async Task CreateAsync(AlbumCreateVm vm)
    {
        var album = mapper.Map<Album>(vm);
        album.Image = await imageService.SaveImageAsync(vm.Image);

        await context.Albums.AddAsync(album);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            imageService.DeleteImageIfExists(album.Image);
            throw;
        }
    }

    public async Task UpdateAsync(AlbumUpdateVm vm)
    {
        var album = await context.Albums.FirstAsync(c => c.Id == vm.Id);

        string oldImage = album.Image;

        album.Name = vm.Name;
        album.Image = await imageService.SaveImageAsync(vm.Image);
        album.ArtistId = vm.ArtistId;

        try
        {
            await context.SaveChangesAsync();

            imageService.DeleteImageIfExists(oldImage);
        }
        catch (Exception)
        {
            imageService.DeleteImageIfExists(album.Image);
            throw;
        }
    }

    public async Task DeleteIfExistsAsync(long id)
    {
        var album = await context.Albums.FirstOrDefaultAsync(c => c.Id == id);

        if (album is null)
            return;

        context.Albums.Remove(album);
        await context.SaveChangesAsync();

        imageService.DeleteImageIfExists(album.Image);
    }
}