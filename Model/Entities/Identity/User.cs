using Microsoft.AspNetCore.Identity;

namespace Model.Entities.Identity;

public class User : IdentityUser<long>
{
    public string Name { get; set; } = null!;
    public string Photo { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = null!;

    public ICollection<Like> Likes { get; set; } = null!;
    public ICollection<Playlist> Playlists { get; set; } = null!;
    public ICollection<Follower> Subscriptions { get; set; } = null!;
}