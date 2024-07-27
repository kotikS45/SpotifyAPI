﻿using FluentValidation;
using SpotifyAPI.Models.Track;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Track;

public class TrackUpdateValidator : AbstractValidator<TrackUpdateVm>
{
    public TrackUpdateValidator(IExistingEntityCheckerService existingEntityCheckerService, IAudioValidator audioValidator)
    {
        RuleFor(a => a.Id)
            .MustAsync(existingEntityCheckerService.IsCorrectTrackId)
                .WithMessage("Track with this id is not exists");

        RuleFor(a => a.Name)
            .NotEmpty()
                .WithMessage("Name is empty or null")
            .MaximumLength(255)
                .WithMessage("Name is too long");

        RuleFor(a => a.Audio)
            .NotNull()
                .WithMessage("Audio is not selected")
            .Must(audioValidator.IsValidAudio)
                .WithMessage("Audio is not valid");

        RuleFor(a => a.AlbumId)
            .MustAsync(existingEntityCheckerService.IsCorrectAlbumId)
                .WithMessage("Album with this if is not exists");

        RuleForEach(a => a.Genres)
            .MustAsync(existingEntityCheckerService.IsCorrectGenreId)
                .WithMessage("One or more genre IDs are invalid");

        RuleFor(a => a.Genres)
            .Must(t => t.Count > 0)
                .WithMessage("The track must have at least one genre");
    }
}