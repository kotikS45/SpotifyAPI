using AutoMapper;
using Model.Entities;
using SpotifyAPI.Models.Artist;
using System.Diagnostics.Metrics;

namespace SpotifyAPI.Mapper;

public class AppMapProfile : Profile
{
    public AppMapProfile()
    {
        CreateMap<Artist, ArtistVm>();
        CreateMap<ArtistCreateVm, Artist>()
            .ForMember(c => c.Image, opt => opt.Ignore());
    }
}