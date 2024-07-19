using AutoMapper;
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
}