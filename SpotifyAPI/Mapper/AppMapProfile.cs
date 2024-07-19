using AutoMapper;
using Model.Entities;
using SpotifyAPI.Models.Album;
using SpotifyAPI.Models.Artist;
using SpotifyAPI.Models.Track;

namespace SpotifyAPI.Mapper;

public class AppMapProfile : Profile
{
    public AppMapProfile()
    {
        CreateMap<Artist, ArtistVm>();
        CreateMap<ArtistCreateVm, Artist>()
            .ForMember(c => c.Image, opt => opt.Ignore());

        CreateMap<Album, AlbumVm>();
        CreateMap<AlbumCreateVm, Album>()
            .ForMember(c => c.Image, opt => opt.Ignore());

        CreateMap<Track, TrackVm>();
        CreateMap<TrackCreateVm, Track>()
            .ForMember(c => c.Path, opt => opt.Ignore())
            .ForMember(C => C.Duration, opt => opt.Ignore());
    }
}