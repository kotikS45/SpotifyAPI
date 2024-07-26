using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.PlaylistTrack;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class PlaylistTrackControllerService(
    DataContext context) : IPlaylistTrackControllerService
{
    public async Task CreateAsync(PlaylistTrackCreateVm vm)
    {
        var playlist = await context.Playlists.Include(x => x.Tracks).FirstAsync(x => x.Id == vm.PlaylistId);

        var track = await context.Tracks.FirstAsync(x => x.Id == vm.TrackId);

        if (!playlist.Tracks.Any(pt => pt.TrackId == vm.TrackId))
        {
            playlist.Tracks.Add(new PlaylistTrack { PlaylistId = playlist.Id, TrackId = track.Id });
        }

        await context.SaveChangesAsync();
    }

    public async Task DeleteIfExistsAsync(PlaylistTrackDeleteVm vm)
    {
        var playlistTrack = await context.PlaylistTracks
            .FirstAsync(x => x.PlaylistId == vm.PlaylistId && x.TrackId == vm.TrackId);

        context.PlaylistTracks.Remove(playlistTrack);

        await context.SaveChangesAsync();
    }
}