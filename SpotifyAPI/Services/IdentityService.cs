using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Model.Entities.Identity;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class IdentityService(
    UserManager<User> userManager
    ) : IIdentityService
{

    public async Task<User> GetCurrentUserAsync(ControllerBase controller)
    {
        string email = controller.User.Claims
                .FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")
                ?.Value
                ?? throw new Exception("User error");

        User user = await userManager
            .FindByEmailAsync(email)
            ?? throw new Exception("User error");

        return user;
    }
}