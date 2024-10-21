using FluentValidation;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Services;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Genre;

public class GenreCreateValidator : AbstractValidator<GenreCreateVm>
{
    public GenreCreateValidator(IImageValidator imageValidator, IExistingEntityCheckerService existingEntityCheckerService)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Name is empty or null")
            .MaximumLength(50)
                .WithMessage("Name is too long")
            .MustAsync(existingEntityCheckerService.IsAvailableGenreName)
                .WithMessage("Genre with this name already exist");

        RuleFor(c => c.Image)
            .NotNull()
                .WithMessage("Image is not selected")
            .MustAsync(imageValidator.IsValidImageAsync)
                .WithMessage("Image is not valid");
    }
}