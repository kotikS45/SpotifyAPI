using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Model.Entities.Identity;
using SpotifyAPI.Models.Identity;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Validators.Account;

public class UpdateValidator : AbstractValidator<UserUpdateVm>
{
    private readonly UserManager<User> _userManager;
    private readonly IImageValidator _imageValidator;

    public UpdateValidator(UserManager<User> userManager, IImageValidator imageValidator)
    {
        _userManager = userManager;
        _imageValidator = imageValidator;

        RuleFor(r => r.Username)
            .MaximumLength(100)
                .WithMessage("Username is too long");

        RuleFor(r => r.Image)
            .MustAsync(IsValidImageAsync)
                .WithMessage("Image is not valid");
    }

    private async Task<bool> IsValidImageAsync(IFormFile image, CancellationToken _)
    {
        if (image is null) return true;
        return await _imageValidator.IsValidImageAsync(image, _);
    }
}