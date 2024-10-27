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

        CreateMap<Artist, ArtistVm>()
            .ForMember(dest => dest.AlbumsCount, opt => opt.MapFrom(src => src.Albums.Count))
            .ForMember(dest => dest.TracksCount, opt => opt.MapFrom(src => src.Albums.SelectMany(a => a.Tracks).Count()));
        CreateMap<ArtistCreateVm, Artist>()
            .ForMember(c => c.Image, opt => opt.Ignore());

        CreateMap<Album, AlbumVm>();
        CreateMap<AlbumCreateVm, Album>()
            .ForMember(c => c.Image, opt => opt.Ignore());

        CreateMap<Track, TrackVm>()
            .ForMember(c => c.ArtistId, opt => opt.MapFrom(src => src.Album.ArtistId))
            .ForMember(c => c.ArtistName, opt => opt.MapFrom(src => src.Album.Artist.Name));
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