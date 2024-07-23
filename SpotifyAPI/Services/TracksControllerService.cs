using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Like;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class TracksControllerService(
    DataContext context,
    IMapper mapper,
    IAudioService audioService) : ITrackControllerService
{
    public async Task CreateAsync(TrackCreateVm vm)
    {
        var track = mapper.Map<Track>(vm);

        track.Path = await audioService.SaveAudioAsync(vm.Audio);
        track.Duration = audioService.GetAudioDuration(track.Path);

        context.Tracks.Add(track);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            audioService.DeleteAudio(track.Path);
            throw;
        }
    }

    public async Task UpdateAsync(TrackUpdateVm vm)
    {
        var track = await context.Tracks.FirstAsync(t => t.Id == vm.Id);

        string oldAudio = track.Path;

        track.Name = vm.Name;
        track.Path = await audioService.SaveAudioAsync(vm.Audio);
        track.Duration = audioService.GetAudioDuration(track.Path);
        track.AlbumId = vm.AlbumId;

        try
        {
            await context.SaveChangesAsync();

            audioService.DeleteAudioIfExists(oldAudio);
        }
        catch (Exception)
        {
            audioService.DeleteAudioIfExists(track.Path);
            throw;
        }
    }

    public async Task DeleteIfExistsAsync(long id)
    {
        var track = await context.Tracks.FirstOrDefaultAsync(c => c.Id == id);

        if (track is null)
            return;

        context.Tracks.Remove(track);
        await context.SaveChangesAsync();

        audioService.DeleteAudioIfExists(track.Path);
    }
}