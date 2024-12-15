using FluentValidation;
using SpotifyAPI.Models.Search;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Search;

public class SearchValidator : AbstractValidator<SearchVm>
{
    public SearchValidator(IExistingEntityCheckerService existingEntityCheckerService)
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrEmpty(x.Search) || x.GenreId.HasValue)
            .WithMessage("Either Search or GenreId must be provided.");

        RuleFor(x => x.Search)
            .MaximumLength(50)
                .WithMessage("Search is too long")
            .MinimumLength(2)
                .When(x => !string.IsNullOrEmpty(x.Search))
                .WithMessage("Search is too short");

        RuleFor(x => x.GenreId)
            .MustAsync(existingEntityCheckerService.IsCorrectGenreId)
                .When(x => x.GenreId.HasValue)
                .WithMessage("Genre not found");

        // Validation for AlbumsCount
        RuleFor(x => x.AlbumsCount)
            .GreaterThanOrEqualTo(0)
                .WithMessage("AlbumsCount cannot be negative")
            .LessThanOrEqualTo(20)
                .WithMessage("AlbumsCount cannot exceed 20");

        // Validation for TracksCount
        RuleFor(x => x.TracksCount)
            .GreaterThanOrEqualTo(0)
                .WithMessage("TracksCount cannot be negative")
            .LessThanOrEqualTo(20)
                .WithMessage("TracksCount cannot exceed 20");

        // Validation for PlaylistsCount
        RuleFor(x => x.PlaylistsCount)
            .GreaterThanOrEqualTo(0)
                .WithMessage("PlaylistsCount cannot be negative")
            .LessThanOrEqualTo(20)
                .WithMessage("PlaylistsCount cannot exceed 20");
    }
}