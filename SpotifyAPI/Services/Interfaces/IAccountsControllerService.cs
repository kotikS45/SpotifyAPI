using Model.Entities.Identity;
using SpotifyAPI.Models.Identity;

namespace SpotifyAPI.Services.Interfaces;

public interface IAccountsControllerService
{
    Task<User> SignUpAsync(RegisterVm vm);
    Task ResetPasswordAsync(string email, string token, string newPassword);
    Task GeneratePasswordResetTokenAsync(string email);
    Task UpdateAsync(UserUpdateVm vm, User user);
}