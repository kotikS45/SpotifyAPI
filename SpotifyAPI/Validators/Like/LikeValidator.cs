using FluentValidation;
using SpotifyAPI.Models.Like;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Like;

public class LikeValidator : AbstractValidator<LikeVm>
{
    public LikeValidator(IExistingEntityCheckerService existingEntityCheckerService)
    {
        RuleFor(x => x.TrackId)
            .MustAsync(existingEntityCheckerService.IsCorrectTrackId)
                .WithMessage("Track with this id is not exists");
    }
}