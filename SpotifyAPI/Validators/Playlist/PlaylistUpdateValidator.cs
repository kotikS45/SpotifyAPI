using FluentValidation;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Artist;

public class PlaylistUpdateValidator : AbstractValidator<PlaylistUpdateVm>
{
    public PlaylistUpdateValidator(IExistingEntityCheckerService existingEntityCheckerService, IImageValidator imageValidator)
    {
        RuleFor(c => c.Id)
            .MustAsync(existingEntityCheckerService.IsCorrectArtistId)
                .WithMessage("Artist with this id is not exists");

        RuleFor(c => c.Name)
            .NotEmpty()
                .WithMessage("Name is empty or null")
            .MaximumLength(255)
                .WithMessage("Name is too long");

        RuleFor(c => c.Image)
            .NotNull()
                .WithMessage("Image is not selected")
            .MustAsync(imageValidator.IsValidImageAsync)
                .WithMessage("Image is not valid");
    }
}