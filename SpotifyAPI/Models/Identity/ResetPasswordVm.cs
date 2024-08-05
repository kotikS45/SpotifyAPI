namespace SpotifyAPI.Models.Identity;

public class ResetPasswordVm
{
    public string Email { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string Password { get; set; } = null!;
}
