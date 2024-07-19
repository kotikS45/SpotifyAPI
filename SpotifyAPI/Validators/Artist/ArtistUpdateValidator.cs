using FluentValidation;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Artist;

public class ArtistUpdateValidator : AbstractValidator<ArtistUpdateVm>
{
    public ArtistUpdateValidator(IExistingEntityCheckerService existingEntityCheckerService, IImageValidator imageValidator)
    {
        RuleFor(c => c.Id)
            .MustAsync(existingEntityCheckerService.IsCorrectArtistId)
                .WithMessage("Artist with this id is not exists");

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