using FluentValidation;
using SpotifyAPI.Models.Follower;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Follower;

public class FollowerValidator : AbstractValidator<FollowerVm>
{
    public FollowerValidator(IExistingEntityCheckerService existingEntityCheckerService)
    {
        RuleFor(x => x.ArtistId)
            .MustAsync(existingEntityCheckerService.IsCorrectArtistId)
                .WithMessage("Artist with this id is not exists");
    }
}