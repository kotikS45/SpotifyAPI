using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Like;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class LikeControllerService(
    DataContext context) : ILikeControllerService
{
    public async Task Like(long userId, LikeVm vm)
    {
        bool alreadyFollowing = await context.Likes
            .AnyAsync(f => f.UserId == userId && f.TrackId == vm.TrackId);

        if (alreadyFollowing)
            return;

        await context.Likes.AddAsync(new Like { UserId = userId, TrackId = vm.TrackId });
        await context.SaveChangesAsync();
    }

    public async Task Unlike(long userId, LikeVm vm)
    {
        var like = await context.Likes
            .FirstOrDefaultAsync(f => f.UserId == userId && f.TrackId == vm.TrackId);

        if (like is null)
            return;

        context.Likes.Remove(like);
        await context.SaveChangesAsync();
    }
}