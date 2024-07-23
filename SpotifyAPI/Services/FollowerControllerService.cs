using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Follower;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class FollowerControllerService(
    DataContext context) : IFollowerControllerService
{
    public async Task Follow(long userId, FollowerVm vm)
    {
        bool alreadyFollowing = await context.Followers
            .AnyAsync(f => f.UserId == userId && f.ArtistId == vm.ArtistId);

        if (alreadyFollowing)
            return;

        await context.Followers.AddAsync(new Follower { UserId = userId, ArtistId = vm.ArtistId });
        await context.SaveChangesAsync();
    }

    public async Task Unfollow(long userId, FollowerVm vm)
    {
        var follower = await context.Followers
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ArtistId == vm.ArtistId);

        if (follower is null)
            return;

        context.Followers.Remove(follower);
        await context.SaveChangesAsync();
    }
}