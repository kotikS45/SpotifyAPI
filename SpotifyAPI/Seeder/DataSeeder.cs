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

    public async Task SeedAsync()
    {
        if (!context.Genres.Any())
            await SeedGenresAsync();

        if (!context.Artists.Any())
            await SeedArtistAsync();
    }

    public async Task SeedGenresAsync()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "genres.json");
        var jsonData = await File.ReadAllTextAsync(filePath);
        var genres = JsonConvert.DeserializeObject<GenresList>(jsonData);

        if (genres is null) return;

        foreach (var genre in genres.Genres)
        {
            context.Genres.Add(new Genre
            {
                Id = genre.Id,
                Name = genre.Name,
                Image = await imageService.SaveImageAsync(genre.Image)
            });
        }

        await context.SaveChangesAsync();
    }

    public async Task SeedArtistAsync()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "seed.json");
        var jsonData = await File.ReadAllTextAsync(filePath);
        var artists = JsonConvert.DeserializeObject<ArtistList>(jsonData);

        if (artists is null) return;

        foreach (var artist in artists.Artists)
        {
            var artistImage = await imageService.SaveImageAsync(artist.Image);

            var artistEntity = new Artist
            {
                Id = artist.Id,
                Name = artist.Name,
                Image = artistImage,
                Albums = new List<Album>()
            };

            foreach (var album in artist.Albums)
            {
                var albumImage = await imageService.SaveImageAsync(album.Image);

                var albumEntity = new Album
                {
                    Id = album.Id,
                    Name = album.Name,
                    Image = albumImage,
                    ReleaseDate = DateTime.Parse(album.ReleaseDate),
                    Tracks = new List<Track>()
                };

                foreach (var track in album.Tracks)
                {
                    var trackImage = await imageService.SaveImageAsync(track.Image);

                    IFormFile audioFile = await GetAudioFileAsync(Path.Combine(Directory.GetCurrentDirectory(), "Data", "seed", track.Path));
                    var trackAudio = await audioService.SaveAudioAsync(audioFile);

                    var trackGenres = new List<TrackGenre>();
                    foreach (var genreName in track.Genres)
                    {
                        var genre = await context.Genres
                            .FirstOrDefaultAsync(g => g.Name.Equals(genreName));

                        if (genre != null)
                        {
                            trackGenres.Add(new TrackGenre
                            {
                                Genre = genre
                            });
                        }
                    }

                    var trackEntity = new Track
                    {
                        Id = track.Id,
                        Name = track.Name,
                        Duration = audioService.GetAudioDuration(trackAudio),
                        Path = trackAudio,
                        Image = trackImage,
                        Genres = trackGenres
                    };


                    albumEntity.Tracks.Add(trackEntity);
                }

                artistEntity.Albums.Add(albumEntity);
            }

            context.Artists.Add(artistEntity);
        }

        await context.SaveChangesAsync();
    }

    public async Task<IFormFile> GetAudioFileAsync(string filePath)
    {
        var fileBytes = await File.ReadAllBytesAsync(filePath);
        var stream = new MemoryStream(fileBytes);
        var formFile = new FormFile(stream, 0, fileBytes.Length, "file", Path.GetFileName(filePath))
        {
            Headers = new HeaderDictionary(),
            ContentType = "audio/mpeg"
        };

        return formFile;
    }
}

class GenresList
{
    public List<GenreJson> Genres { get; set; } = null!;
}

class GenreJson
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
}

class ArtistList
{
    public List<ArtistJson> Artists { get; set; } = null!;
}

class ArtistJson
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public List<AlbumJson> Albums { get; set; } = null!;
}

class AlbumJson
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string Image { get; set; } = null!;
    public string ReleaseDate { get; set; } = null!;
    public List<TrackJson> Tracks { get; set; } = null!;
}

class TrackJson
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public int Duration { get; set; }
    public string Path { get; set; } = null!;
    public string Image { get; set; } = null!;
    public List<string> Genres { get; set; } = null!;
}