using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Model.Context;
using Model.Entities.Identity;
using SpotifyAPI.Constants;
using SpotifyAPI.Exceptions;
using SpotifyAPI.Models.Identity;
using SpotifyAPI.Services.Interfaces;
using SpotifyAPI.SMTP;

namespace SpotifyAPI.Services;

public class AccountsControllerService(
    DataContext context,
    UserManager<User> userManager,
    IMapper mapper,
    IImageService imageService,
    IConfiguration configuration,
    IEmailService emailService
    ) : IAccountsControllerService
{

    public async Task<User> SignUpAsync(RegisterVm vm)
    {
        User user = mapper.Map<RegisterVm, User>(vm);
        user.Photo = await imageService.SaveImageAsync(vm.Image);

        try
        {
            await CreateUserAsync(user, vm.Password);
        }
        catch
        {
            imageService.DeleteImageIfExists(user.Photo);
            throw;
        }

        return user;
    }

    private async Task CreateUserAsync(User user, string? password = null)
    {
        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            IdentityResult identityResult = await CreateUserInDatabaseAsync(user, password);
            if (!identityResult.Succeeded)
                throw new IdentityException(identityResult, "User creating error");

            identityResult = await userManager.AddToRoleAsync(user, Roles.User);
            if (!identityResult.Succeeded)
                throw new IdentityException(identityResult, "Role assignment error");

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<IdentityResult> CreateUserInDatabaseAsync(User user, string? password)
    {
        if (password is null)
            return await userManager.CreateAsync(user);

        return await userManager.CreateAsync(user, password);
    }

    public async Task ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return;
        }

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Password reset failed: {errors}");
        }
    }

    public async Task GeneratePasswordResetTokenAsync(string email)
    {
        var url = configuration["PasswordReset:CallbackUrl"]
               ?? throw new NullReferenceException("PasswordReset:CallbackUrl");

        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return;
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        var callbackUrl = $"{url}?token={token}&email={email}";

        var message = new Message()
        {
            To = user.Email,
            Name = user.UserName,
            Body = $"Please reset your password by clicking here: <a href='{callbackUrl}'>link</a>"
        };

        await emailService.SendAsync(message);
    }
}