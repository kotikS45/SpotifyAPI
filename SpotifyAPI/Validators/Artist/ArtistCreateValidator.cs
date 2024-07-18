using FluentValidation;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Artist;

public class ArtistCreateValidator : AbstractValidator<ArtistCreateVm>
{
    public ArtistCreateValidator(IImageValidator imageValidator)
    {
        RuleFor(c => c.Name)
            .NotEmpty()
                .WithMessage("Name is empty or null")
            .MaximumLength(255)
                .WithMessage("Name is too long");

        RuleFor(c => c.Genre)
            .NotEmpty()
                .WithMessage("Genre is empty or null")
            .MaximumLength(255)
                .WithMessage("Genre is too long");

        RuleFor(c => c.Image)
            .NotNull()
                .WithMessage("Image is not selected")
            .MustAsync(imageValidator.IsValidImageAsync)
                .WithMessage("Image is not valid");
    }
}