using AutoMapper;
using MailKit;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class TracksControllerService(
    DataContext context,
    IMapper mapper,
    IAudioService audioService,
    IImageService imageService) : ITrackControllerService
{
    public async Task CreateAsync(TrackCreateVm vm)
    {
        var track = mapper.Map<Track>(vm);

        track.Path = await audioService.SaveAudioAsync(vm.Audio);
        track.Duration = audioService.GetAudioDuration(track.Path);
        track.Image = await imageService.SaveImageAsync(vm.Image);

        await context.Tracks.AddAsync(track);

        try
        {
            await context.SaveChangesAsync();

            foreach (var genreId in vm.Genres)
            {
                context.TrackGenres.Add(new TrackGenre
                {
                    TrackId = track.Id,
                    GenreId = genreId
                });
            }

            await context.SaveChangesAsync();
        }
        catch (Exception)
        {
            imageService.DeleteImageIfExists(track.Image);
            audioService.DeleteAudio(track.Path);
            throw;
        }
    }

    public async Task UpdateAsync(TrackUpdateVm vm)
    {
        var track = await context.Tracks.FirstAsync(t => t.Id == vm.Id);

        string oldAudio = track.Path;
        string oldImage = track.Image;

        track.Name = vm.Name;
        track.Path = await audioService.SaveAudioAsync(vm.Audio);
        track.Image = await imageService.SaveImageAsync(vm.Image);
        track.Duration = audioService.GetAudioDuration(track.Path);
        track.AlbumId = vm.AlbumId;

        var genres = await context.TrackGenres.Where(x => x.TrackId == track.Id).ToArrayAsync();
        context.TrackGenres.RemoveRange(genres);

        foreach (var genreId in vm.Genres)
        {
            context.TrackGenres.Add(new TrackGenre
            {
                TrackId = track.Id,
                GenreId = genreId
            });
        }

        try
        {
            await context.SaveChangesAsync();

            audioService.DeleteAudioIfExists(oldAudio);

            imageService.DeleteImageIfExists(oldImage);
        }
        catch (Exception)
        {
            audioService.DeleteAudioIfExists(track.Path);
            imageService.DeleteImageIfExists(track.Image);
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
        imageService.DeleteImageIfExists(track.Image);
    }
}