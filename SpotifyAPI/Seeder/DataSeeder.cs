using Bogus;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using SpotifyAPI.Seeder.Interfaces;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Seeder;

public class DataSeeder(
    DataContext context,
    IImageService imageService,
    IAudioService audioService
    ) : IDataSeeder
{
    public async Task SeedAsync()
    {
        if (!await context.Artists.AnyAsync())
            await CreateArtistsAsync();

    }

    public async Task CreateArtistsAsync()
    {
        Faker faker = new Faker();
        using var httpClient = new HttpClient();

        var artists = new List<Artist>();
        for (int i = 0; i < 50; i++)
        {
            var imageUrl = faker.Internet.Avatar();
            var base64 = await GetImageAsBase64Async(httpClient, imageUrl);

            var artist = new Artist
            {
                Name = faker.Name.FullName(),
                Image = await imageService.SaveImageAsync(base64)
            };

            artists.Add(artist);
        }

        context.AddRange(artists);
        context.SaveChanges();
    }


    private static async Task<string> GetImageAsBase64Async(HttpClient httpClient, string imageUrl)
    {
        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
        return Convert.ToBase64String(imageBytes);
    }
}