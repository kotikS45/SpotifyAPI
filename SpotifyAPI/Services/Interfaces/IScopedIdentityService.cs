using Microsoft.AspNetCore.Mvc;
using Model.Entities.Identity;

namespace SpotifyAPI.Services.Interfaces;

public interface IScopedIdentityService
{
    User? User { get; }

    Task InitCurrentUserAsync(ControllerBase controller);

    User GetRequiredUser();

    long GetRequiredUserId();
}
