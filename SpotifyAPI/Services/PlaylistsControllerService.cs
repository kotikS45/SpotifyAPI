using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class PlaylistsControllerService(
    DataContext context,
    IMapper mapper,
    IImageService imageService) : IPlaylistControllerService
{
    public async Task CreateAsync(PlaylistCreateVm vm, long userId)
    {
        var playlist = mapper.Map<Playlist>(vm);
        playlist.Image = await imageService.SaveImageAsync(vm.Image);
        playlist.UserId = userId;

        await context.Playlists.AddAsync(playlist);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            imageService.DeleteImageIfExists(playlist.Image);
            throw;
        }
    }

    public async Task UpdateAsync(PlaylistUpdateVm vm)
    {
        var playlist = await context.Playlists.FirstAsync(p => p.Id == vm.Id);

        playlist.Name = vm.Name;

        string oldImage = playlist.Image;
        playlist.Image = await imageService.SaveImageAsync(vm.Image);

        try
        {
            await context.SaveChangesAsync();

            imageService.DeleteImageIfExists(oldImage);
        }
        catch (Exception)
        {
            imageService.DeleteImageIfExists(playlist.Image);
            throw;
        }
    }

    public async Task DeleteIfExistsAsync(long id)
    {
        var playlist = await context.Playlists.FirstOrDefaultAsync(p => p.Id == id);

        if (playlist is null)
        {
            return;
        }

        context.Playlists.Remove(playlist);
        await context.SaveChangesAsync();

        imageService.DeleteImageIfExists(playlist.Image);
    }
}