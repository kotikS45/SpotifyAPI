using FluentValidation;
using SpotifyAPI.Models.PlaylistTrack;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.PlaylistTrack;

public class PlaylistTrackCreateValidator : AbstractValidator<PlaylistTrackCreateVm>
{
    public PlaylistTrackCreateValidator(IExistingEntityCheckerService existingEntityCheckerService)
    {
        RuleFor(x => x.PlaylistId)
            .MustAsync(existingEntityCheckerService.IsCorrectPlaylistId)
                .WithMessage("Playlist with this if is not exists");

        RuleFor(x => x.TrackId)
            .MustAsync(existingEntityCheckerService.IsCorrectTrackId)
                .WithMessage("Track with this if is not exists");
    }
}