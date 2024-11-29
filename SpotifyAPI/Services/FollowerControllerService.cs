using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Follower;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class FollowerControllerService(
    DataContext context) : IFollowerControllerService
{
    public async Task Follow(long userId, long artistId)
    {
        bool alreadyFollowing = await context.Followers
            .AnyAsync(f => f.UserId == userId && f.ArtistId == artistId);

        if (alreadyFollowing)
            return;

        await context.Followers.AddAsync(new Follower { UserId = userId, ArtistId = artistId });
        await context.SaveChangesAsync();
    }

    public async Task Unfollow(long userId, long artistId)
    {
        var follower = await context.Followers
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ArtistId == artistId);

        if (follower is null)
            return;

        context.Followers.Remove(follower);
        await context.SaveChangesAsync();
    }
}