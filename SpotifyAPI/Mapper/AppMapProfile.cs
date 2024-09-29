using AutoMapper;
using Model.Entities;
using Model.Entities.Identity;
using SpotifyAPI.Models.Album;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Models.Identity;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Mapper;

public class AppMapProfile : Profile
{
    public AppMapProfile()
    {
        CreateMap<RegisterVm, User>();
        CreateMap<User, UserVm>();

        CreateMap<Artist, ArtistVm>();
        CreateMap<ArtistCreateVm, Artist>()
            .ForMember(c => c.Image, opt => opt.Ignore());

        CreateMap<Album, AlbumVm>();
        CreateMap<AlbumCreateVm, Album>()
            .ForMember(c => c.Image, opt => opt.Ignore());

        CreateMap<Track, TrackVm>();
        CreateMap<TrackCreateVm, Track>()
            .ForMember(c => c.Path, opt => opt.Ignore())
            .ForMember(C => C.Duration, opt => opt.Ignore())
            .ForMember(c => c.Genres, opt => opt.Ignore());

        CreateMap<Playlist, PlaylistVm>();
        CreateMap<PlaylistCreateVm, Playlist>()
            .ForMember(c => c.Image, opt => opt.Ignore());

        CreateMap<Genre, GenreVm>();
        CreateMap<GenreCreateVm, Genre>();

        CreateMap<TrackGenre, GenreVm>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Genre.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Genre.Name));
    }
}
/*
using AutoMapper;
using Model.Entities;
using Model.Entities.Identity;
using SpotifyAPI.Models.Album;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Genre;
using SpotifyAPI.Models.Identity;
using SpotifyAPI.Models.Playlist;
using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Mapper;

public class AppMapProfile : Profile
{
    public AppMapProfile()
    {
        // Маппінг для користувачів
        CreateMap<RegisterVm, User>();
        CreateMap<User, UserVm>();

        // Маппінг для артистів з обробкою зображень
        CreateMap<Artist, ArtistVm>();
        CreateMap<ArtistCreateVm, Artist>()
            .ForMember(c => c.Image, opt => opt.MapFrom(src => src.Image ?? GetDefaultArtistImage()));

        // Маппінг для альбомів з обробкою зображень
        CreateMap<Album, AlbumVm>();
        CreateMap<AlbumCreateVm, Album>()
            .ForMember(c => c.Image, opt => opt.MapFrom(src => src.Image ?? GetDefaultAlbumImage()));

        // Маппінг для треків з обчисленням тривалості та шляхом до файлу
        CreateMap<Track, TrackVm>();
        CreateMap<TrackCreateVm, Track>()
            .ForMember(c => c.Path, opt => opt.MapFrom(src => GenerateFilePath(src.File)))
            .ForMember(c => c.Duration, opt => opt.MapFrom(src => CalculateDuration(src.File)))
            .ForMember(c => c.Genres, opt => opt.MapFrom(src => MapGenres(src.GenreIds)));

        // Маппінг для плейлістів з обробкою зображень
        CreateMap<Playlist, PlaylistVm>();
        CreateMap<PlaylistCreateVm, Playlist>()
            .ForMember(c => c.Image, opt => opt.MapFrom(src => src.Image ?? GetDefaultPlaylistImage()));

        // Маппінг для жанрів
        CreateMap<Genre, GenreVm>();
        CreateMap<GenreCreateVm, Genre>();

        // Маппінг для жанрів треку
        CreateMap<TrackGenre, GenreVm>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Genre.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Genre.Name));
    }

    // Метод для отримання шляху до файлу
    private string GenerateFilePath(string file)
    {
        return $"/uploads/tracks/{Guid.NewGuid()}_{file}";
    }

    // Метод для обчислення тривалості треку
    private TimeSpan CalculateDuration(string file)
    {
        // Логіка для обчислення тривалості
        return new TimeSpan(0, 3, 45); // наприклад, 3 хвилини 45 секунд
    }

    // Метод для отримання зображення за замовчуванням
    private string GetDefaultArtistImage()
    {
        return "/images/default-artist.png";
    }

    private string GetDefaultAlbumImage()
    {
        return "/images/default-album.png";
    }

    private string GetDefaultPlaylistImage()
    {
        return "/images/default-playlist.png";
    }

    // Метод для маппінгу ідентифікаторів жанрів у об'єкти жанрів
    private ICollection<Genre> MapGenres(IEnumerable<int> genreIds)
    {
        // Логіка для пошуку жанрів по ID
        return new List<Genre>(); // повернути список жанрів
    }
}

*/
