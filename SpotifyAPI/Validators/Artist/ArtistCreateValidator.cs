using FluentValidation;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Artist;

public class PlaylistCreateValidator : AbstractValidator<ArtistCreateVm>
{
    public PlaylistCreateValidator(IImageValidator imageValidator)
    {
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