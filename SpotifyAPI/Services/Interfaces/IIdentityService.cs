using Microsoft.AspNetCore.Mvc;
using Model.Entities.Identity;

namespace SpotifyAPI.Services.Interfaces;

public interface IIdentityService
{
    Task<User> GetCurrentUserAsync(ControllerBase controller);
}