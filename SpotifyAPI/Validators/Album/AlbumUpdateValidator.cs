using FluentValidation;
using SpotifyAPI.Models.Album;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Album;

public class AlbumUpdateValidator : AbstractValidator<AlbumUpdateVm>
{
    public AlbumUpdateValidator(IExistingEntityCheckerService existingEntityCheckerService, IImageValidator imageValidator)
    {
        RuleFor(a => a.Id)
            .MustAsync(existingEntityCheckerService.IsCorrectAlbumId)
                .WithMessage("Album with this id is not exists");

        RuleFor(a => a.Name)
            .NotEmpty()
                .WithMessage("Name is empty or null")
            .MaximumLength(255)
                .WithMessage("Name is too long");

        RuleFor(a => a.Image)
            .NotNull()
                .WithMessage("Image is not selected")
            .MustAsync(imageValidator.IsValidImageAsync)
                .WithMessage("Image is not valid");

        RuleFor(a => a.ArtistId)
            .MustAsync(existingEntityCheckerService.IsCorrectArtistId)
                .WithMessage("Artist with this id is not exists");
    }
}