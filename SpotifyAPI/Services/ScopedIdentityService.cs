using Microsoft.AspNetCore.Mvc;
using Model.Entities.Identity;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class ScopedIdentityService(
    IIdentityService identityService
) : IScopedIdentityService
{

    public User? User { get; private set; } = null;

    public async Task InitCurrentUserAsync(ControllerBase controller)
    {
        User = await identityService.GetCurrentUserAsync(controller);
    }

    public User GetRequiredUser() =>
        User ?? throw new Exception($"User in {nameof(ScopedIdentityService)} is not inicialized");

    public long GetRequiredUserId() => GetRequiredUser().Id;
}