using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Model.Configurations;
using Model.Configurations.Identity;
using Model.Entities;
using Model.Entities.Identity;

namespace Model.Context;

public class DataContext(DbContextOptions<DataContext> options)
    : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>,
        IdentityRoleClaim<long>, IdentityUserToken<long>>(options)
{
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Playlist> Playlists { get; set; }
    public DbSet<PlaylistTrack> PlaylistTracks { get; set; }
    public DbSet<Follower> Followers { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<TrackGenre> TrackGenres { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        new UserConfiguration().Configure(builder.Entity<User>());
        new UserRoleConfiguration().Configure(builder.Entity<UserRole>());

        new ArtistConfiguration().Configure(builder.Entity<Artist>());
        new AlbumConfiguration().Configure(builder.Entity<Album>());
        new TrackConfiguration().Configure(builder.Entity<Track>());
        new PlaylistConfiguration().Configure(builder.Entity<Playlist>());
        new PlaylistTrackConfiguration().Configure(builder.Entity<PlaylistTrack>());
        new FollowerConfiguration().Configure(builder.Entity<Follower>());
        new LikeConfiguration().Configure(builder.Entity<Like>());
        new GenreConfiguration().Configure(builder.Entity<Genre>());
        new TrackGenreConfiguration().Configure(builder.Entity<TrackGenre>());
    }
}