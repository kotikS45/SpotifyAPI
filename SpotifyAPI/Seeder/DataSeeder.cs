using Bogus;
using Microsoft.EntityFrameworkCore;
using Model.Context;
using Model.Entities;
using Newtonsoft.Json;
using SpotifyAPI.Seeder.Interfaces;
using SpotifyAPI.Services.Interfaces;

namespace SpotifyAPI.Seeder;

public class DataSeeder(
    DataContext context,
    IImageService imageService,
    IAudioService audioService
    ) : IDataSeeder
{
    string apiKey = "2d375f10";

    public async Task SeedAsync()
    {
        if (!await context.Genres.AnyAsync())
            await CreateGenresAsync();

        if (!await context.Artists.AnyAsync())
            await CreateArtistsAsync();

        if (!await context.Playlists.AnyAsync())
            await CreatePlaylistsAsync();
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

        await CreateTracksAsync(albums);
    }

    public async Task CreateTracksAsync(IList<Album> albums)
    {
        Faker faker = new Faker();
        using var httpClient = new HttpClient();
        var genres = await context.Genres.ToListAsync();

        var tracks = new List<Track>();

        foreach (var album in albums)
        {
            int count = faker.Random.Int(1, 10);

            List<string> urls = await GetTrackUrlsAsync(count);

            for (int i = 0; i < count; i++)
            {
                var audioBytes = await httpClient.GetByteArrayAsync(urls[i]);
                var audioPath = await audioService.SaveAudioAsync(audioBytes);

                var track = new Track
                {
                    Name = faker.Lorem.Sentence(),
                    Path = await audioService.SaveAudioAsync(audioBytes),
                    Duration = audioService.GetAudioDuration(audioPath),
                    AlbumId = album.Id
                };
                int genreCount = faker.Random.Int(1, 3);
                var selectedGenres = faker.PickRandom(genres, genreCount).ToList();

                track.Genres = selectedGenres.Select(g => new TrackGenre
                {
                    GenreId = g.Id,
                    Track = track
                }).ToList();

                tracks.Add(track);
            }
        }

        await context.Tracks.AddRangeAsync(tracks);
        await context.SaveChangesAsync();
    }
    public async Task<List<string>> GetTrackIdsAsync(int count)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync($"https://api.jamendo.com/v3.0/tracks/?client_id={apiKey}&format=json&limit={count}");
        var json = await response.Content.ReadAsStringAsync();

        var trackIds = new List<string>();
        dynamic? data = JsonConvert.DeserializeObject(json);

        if (data != null)
        {
            foreach (var track in data.results)
            {
                if (track.id != null)
                {
                    trackIds.Add((string) track.id);
                }
            }
        }

        return trackIds;
    }

    public async Task<List<string>> GetTrackUrlsAsync(int count)
    {
        using var httpClient = new HttpClient();
        var trackIds = await GetTrackIdsAsync(count);
        var trackUrls = new List<string>();

        foreach (var trackId in trackIds)
        {
            var fileUrl = $"https://api.jamendo.com/v3.0/tracks/file/?client_id={apiKey}&id={trackId}";
            if (!string.IsNullOrEmpty(fileUrl))
            {
                trackUrls.Add(fileUrl);
            }
        }

        return trackUrls;
    }

    public async Task CreatePlaylistsAsync()
    {
        Faker faker = new Faker();
        using var httpClient = new HttpClient();

        var playlists = new List<Playlist>();
        for (int i = 0; i < 10; i++)
        {
            var imageUrl = faker.Image.LoremFlickrUrl(keywords: "playlist");
            var base64 = await GetImageAsBase64Async(httpClient, imageUrl);

            var playlist = new Playlist
            {
                Name = faker.Name.FullName(),
                Image = await imageService.SaveImageAsync(base64),
                UserId = (await context.Users.FirstAsync(x => x.UserName == "admin")).Id,
            };

            playlists.Add(playlist);
        }

        await context.AddRangeAsync(playlists);
        await context.SaveChangesAsync();

        await AddTracksToPlaylistsAsync(playlists);
    }

    public async Task AddTracksToPlaylistsAsync(IList<Playlist> playlists)
    {
        Faker faker = new Faker();
        var tracks = await context.Tracks.ToListAsync();

        var playlistTracks = new List<PlaylistTrack>();

        foreach (var playlist in playlists)
        {
            int trackCount = faker.Random.Int(10, 20);

            var selectedTracks = faker.PickRandom(tracks, trackCount);

            foreach (var track in selectedTracks)
            {
                playlistTracks.Add(new PlaylistTrack
                {
                    PlaylistId = playlist.Id,
                    TrackId = track.Id
                });
            }
        }

        await context.AddRangeAsync(playlistTracks);
        await context.SaveChangesAsync();
    }

    private static async Task<string> GetImageAsBase64Async(HttpClient httpClient, string imageUrl)
    {
        var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
        return Convert.ToBase64String(imageBytes);
    }
}