namespace SpotifyAPI.Models.Identity;

public class RegisterVm
{
    public IFormFile Image { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
}