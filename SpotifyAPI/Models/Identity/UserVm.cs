namespace SpotifyAPI.Models.Identity
{
    public class UserVm
    {
        public string Name { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public string NormalizedEmail { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string NormalizedUserName { get; set; } = null!;
        public bool EmailConfirmed { get; set; }
    }
}