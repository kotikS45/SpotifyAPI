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
        if (!await context.Genres.AnyAsync())
            await CreateGenresAsync();

        if (!await context.Artists.AnyAsync())
            await CreateArtistsAsync();
    }

    public async Task CreateGenresAsync()
    {
        var genres = new List<Genre>
        {
            new Genre { Name = "Rock" },
            new Genre { Name = "Pop" },
            new Genre { Name = "Jazz" },
            new Genre { Name = "Classical" },
            new Genre { Name = "Hip-Hop" },
            new Genre { Name = "Electronic" },
            new Genre { Name = "Country" },
            new Genre { Name = "Reggae" },
            new Genre { Name = "Blues" },
            new Genre { Name = "Metal" }
        };

        await context.Genres.AddRangeAsync(genres);
        await context.SaveChangesAsync();
    }

    public async Task CreateArtistsAsync()
    {
        Faker faker = new Faker();
        using var httpClient = new HttpClient();

        var artists = new List<Artist>();
        for (int i = 0; i < 5; i++)
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

        await context.AddRangeAsync(artists);
        await context.SaveChangesAsync();

        await CreateAlbumsAsync(artists);
    }

    public async Task CreateAlbumsAsync(IList<Artist> artists)
    {
        Faker faker = new Faker();
        using var httpClient = new HttpClient();

        var albums = new List<Album>();

        foreach (var artist in artists)
        {
            int count = faker.Random.Int(1, 10);

            for (int i = 0; i < count; i++)
            {
                var imageUrl = faker.Image.LoremFlickrUrl(keywords: "album");
                var base64 = await GetImageAsBase64Async(httpClient, imageUrl);

                var album = new Album
                {
                    Name = faker.Lorem.Sentence(),
                    ReleaseDate = faker.Date.Past(10),
                    Image = await imageService.SaveImageAsync(base64),
                    ArtistId = artist.Id
                };

                albums.Add(album);
            }
        }

        await context.Albums.AddRangeAsync(albums);
        await context.SaveChangesAsync();
    }

    private static async Task<string> GetImageAsBase64Async(HttpClient httpClient, string imageUrl)
    {
        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
        return Convert.ToBase64String(imageBytes);
    }
}