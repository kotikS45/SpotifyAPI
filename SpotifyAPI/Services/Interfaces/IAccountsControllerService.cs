using Model.Entities.Identity;
using SpotifyAPI.Models.Identity;

namespace SpotifyAPI.Services.Interfaces;

public interface IAccountsControllerService
{
    Task<User> SignUpAsync(RegisterVm vm);
}