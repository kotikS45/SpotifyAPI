using AutoMapper;
using Model.Entities;
using SpotifyAPI.Models.Artist;

namespace SpotifyAPI.Mapper;

public class AppMapProfile : Profile
{
    public AppMapProfile()
    {
        CreateMap<Artist, ArtistVm>();
    }
}