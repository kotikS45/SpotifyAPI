using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Models.Like;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Services;

public class LikeControllerService(
    DataContext context) : ILikeControllerService
{
    public async Task Like(long userId, long id)
    {
        bool liked = await context.Likes
            .AnyAsync(f => f.UserId == userId && f.TrackId == id);

        if (liked)
            return;

        await context.Likes.AddAsync(new Like { UserId = userId, TrackId = id});
        await context.SaveChangesAsync();
    }

    public async Task Unlike(long userId, long id)
    {
        var like = await context.Likes
            .FirstOrDefaultAsync(f => f.UserId == userId && f.TrackId == id);

        if (like is null)
            return;

        context.Likes.Remove(like);
        await context.SaveChangesAsync();
    }
}