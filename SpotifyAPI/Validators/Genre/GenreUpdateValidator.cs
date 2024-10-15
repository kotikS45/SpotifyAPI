using FluentValidation;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Services;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Genre;

public class GenreUpdateValidator : AbstractValidator<GenreUpdateVm>
{
    public GenreUpdateValidator(IImageValidator imageValidator, IExistingEntityCheckerService existingEntityCheckerService)
    {
        RuleFor(x => x.Id)
            .MustAsync(existingEntityCheckerService.IsCorrectGenreId)
                .WithMessage("Genre with this id is not exists");

        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Name is empty or null")
            .MaximumLength(50)
                .WithMessage("Name is too long");

        RuleFor(c => c.Image)
            .NotNull()
                .WithMessage("Image is not selected")
            .MustAsync(imageValidator.IsValidImageAsync)
                .WithMessage("Image is not valid");
    }
}